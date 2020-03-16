using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// Jwt签名、解析帮助类
    /// </summary>
    public class AuthHelper
    {
        /// <summary>
        /// Cookie认证
        /// </summary>
        public const string CookieAuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        /// <summary>
        /// Cookie认证Cookie名称
        /// </summary>
        public static string CookieAuthName = ConfigHelper.CookieAuthName;

        /// <summary>
        /// Jwt认证
        /// </summary>
        public const string JwtAuthScheme = JwtBearerDefaults.AuthenticationScheme;

        /// <summary>
        /// Jwt认证Cookie名称
        /// </summary>
        public static string JwtAuthName = ConfigHelper.JwtAuthName;

        /// <summary>
        /// Id4 Oidc认证
        /// </summary>
        public const string Id4OidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

        /// <summary>
        /// 生成JwtToken
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static async Task<LoginParam> GetJwtToken(TokenModelJwt tokenModel)
        {
            string iss = ConfigHelper.JwtIssuer;
            string aud = ConfigHelper.JwtAudience;
            string secret = ConfigHelper.JwtSecret;
            int overSec = ConfigHelper.JwtOverSec;
            DateTime dtNow = DateTime.Now;

            List<Claim> claims = GetJwtClaim(tokenModel, iss, aud, overSec, dtNow);

            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(issuer: iss, claims: claims, signingCredentials: creds);
            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            // 保存用户信息到HttpContext.User
            await IdentifyLogin(claims);

            //// 屏蔽，现在Mvc跳转有Cookies认证，没必要保存Jwt Cookie
            //// Jwt授权保存到Cookies
            //StaticHttpContext.Current.Response.Cookies.Append(AuthHelper.JwtAuthName, encodedJwt,
            //    new CookieOptions
            //    {
            //        HttpOnly = true,
            //        Expires = new DateTimeOffset(dtNow).AddSeconds(overSec)
            //    });

            return new LoginParam
            {
                Token = encodedJwt,
                ExpiresIn = overSec,
                ExpireTimeStamp = new DateTimeOffset(dtNow.AddSeconds(overSec)).ToUnixTimeMilliseconds(),
                UserName = tokenModel.UserName
            };
        }

        /// <summary>
        /// 获取Jwt Claims
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <param name="iss"></param>
        /// <param name="aud"></param>
        /// <param name="overSec"></param>
        /// <param name="dtNow"></param>
        /// <returns></returns>
        private static List<Claim> GetJwtClaim(TokenModelJwt tokenModel, string iss, string aud, int overSec, DateTime dtNow)
        {
            DateTimeOffset dtOffset = new DateTimeOffset(dtNow);
            var claims = new List<Claim>
            {
                //特别重要：
                //  1、这里将用户的部分信息，比如 uid 存到了Claim 中，如果你想知道如何在其他地方将这个 uid从 Token 中取出来，请看下边的SerializeJwt() 方法，
                //     或者在整个解决方案，搜索这个方法，看哪里使用了！
                //  2、你也可以研究下 HttpContext.User.Claims ，具体的你可以看看 Policys/PermissionHandler.cs 类中是如何使用的。
                new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss,iss),
                new Claim(JwtRegisteredClaimNames.Aud,aud),
                new Claim(ClaimTypes.Name,tokenModel.UserName),

                // Jwt时间和超时设置
                new Claim(JwtRegisteredClaimNames.Iat, $"{dtOffset.ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{dtOffset.ToUnixTimeSeconds()}") ,
                //这个就是过期时间，目前是过期1000秒，可自定义，注意JWT有自己的缓冲过期时间
                new Claim(JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(dtNow.AddSeconds(overSec)).ToUnixTimeSeconds()}"),
                new Claim(ClaimTypes.Expiration, dtNow.AddSeconds(overSec).ToString()),
            };

            // 可以将一个用户的多个角色全部赋予；
            // 作者：DX 提供技术支持；
            claims.AddRange(tokenModel.Role.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));
            return claims;
        }

        /// <summary>
        /// 解析JwtToken
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenModelJwt DecryptJwtToken(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object role;
            object userName;
            try
            {
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
                jwtToken.Payload.TryGetValue(ClaimTypes.Name, out userName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var token = new TokenModelJwt
            {
                Uid = (jwtToken.Id).ObjToInt(),
                Role = role != null ? role.ObjToString() : "",
                UserName = userName != null ? userName.ObjToString() : ""
            };

            return token;
        }

        /// <summary>
        /// 保存用户信息到HttpContext.User
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static async Task IdentifyLogin(List<Claim> claims)
        {
            //// 设置Jwt认证(后台暂时没法logout，只能前台Header清空Authorization)
            //await Task.Delay(10);

            //// Jwt授权无法保存到线程和Httpconxt里面，只能每次从Header获取并解析，也没法SignInAsync
            //var jwtIdentity = new ClaimsIdentity(claims, JwtHelper.JwtAuthScheme);
            //var jwtPrincipal = new ClaimsPrincipal(jwtIdentity);
            //StaticHttpContext.Current.User = jwtPrincipal;
            //Thread.CurrentPrincipal = jwtPrincipal;
            //await StaticHttpContext.Current.SignInAsync(JwtHelper.JwtAuthScheme, new ClaimsPrincipal(jwtPrincipal));

            // 设置Cookie认证
            var cookieIdentity = new ClaimsIdentity(claims, AuthHelper.CookieAuthScheme);
            var cookiePrincipal = new ClaimsPrincipal(cookieIdentity);
            StaticHttpContext.Current.User = cookiePrincipal;
            Thread.CurrentPrincipal = cookiePrincipal;
            await StaticHttpContext.Current.SignInAsync(CookieAuthScheme, new ClaimsPrincipal(cookiePrincipal));
        }

        /// <summary>
        /// 认证登出
        /// </summary>
        /// <returns></returns>
        public static async Task IdentifyLogout()
        {
            if (ConfigHelper.IsOidcAuth)
            {
                await StaticHttpContext.Current.SignOutAsync(AuthHelper.CookieAuthScheme);
                await StaticHttpContext.Current.SignOutAsync(AuthHelper.Id4OidcScheme);
            }
            else
            {
                // 屏蔽，现在Mvc跳转有Cookies认证，没必要保存Jwt Cookie
                // 清空Jwt Cookie和Claims
                //await Task.Delay(10);
                //await StaticHttpContext.Current.SignOutAsync(AuthHelper.JwtAuthScheme);
                //StaticHttpContext.Current.Response.Cookies.Delete(AuthHelper.JwtAuthName);

                // 清空Cookie认证
                await StaticHttpContext.Current.SignOutAsync(CookieAuthScheme);
            }
        }
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModelJwt
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Uid { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 职能
        /// </summary>
        public string Work { get; set; }
    }

    /// <summary>
    /// 登录参数
    /// </summary>
    public class LoginParam
    {
        /// <summary>
        /// JwtToken
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 超时秒数
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 超时时间戳
        /// </summary>
        public long ExpireTimeStamp { get; set; }
    }
}
