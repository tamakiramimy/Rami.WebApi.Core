using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 全局用户信息
    /// </summary>
    public class AspNetUser : IUser
    {
        /// <summary>
        /// IHttpContextAccessor
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// 是否Oidc授权模式
        /// </summary>
        public bool IsOidcAuth = ComHelper.GetConf("AppSettings:IsOidcAuth").ObjToBool();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="accessor"></param>
        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get
            {
                if (IsOidcAuth)
                {
                    // IdentityModel.JwtClaimTypes.Name
                    return GetClaimValueByType("name").FirstOrDefault();
                }
                else
                {
                    return GetClaimValueByType(ClaimTypes.Name).FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<string> LstRoles
        {
            get
            {
                if (IsOidcAuth)
                {
                    // IdentityModel.JwtClaimTypes.Role
                    return GetClaimValueByType("role");
                }
                else
                {
                    return GetClaimValueByType(ClaimTypes.Role);
                }
            }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Roles => LstRoles?.Count > 0 ? string.Join(",", LstRoles) : string.Empty;

        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get
            {
                if (IsOidcAuth)
                {
                    // IdentityModel.JwtClaimTypes.Subject
                    return GetClaimValueByType("sub").ObjToInt();
                }
                else
                {
                    return GetClaimValueByType(JwtRegisteredClaimNames.Jti).FirstOrDefault().ObjToInt();
                }
            }
        }

        /// <summary>
        /// 是否授权
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// 用户认证 Claim
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }

        /// <summary>
        /// 根据 Claim 获取值
        /// </summary>
        /// <param name="ClaimType"></param>
        /// <returns></returns>
        public List<string> GetClaimValueByType(string ClaimType)
        {
            List<string> lstClaim = GetClaimsIdentity().Where(x => x.Type == ClaimType).Select(x => x.Value).ToList();
            return lstClaim;
        }
    }
}
