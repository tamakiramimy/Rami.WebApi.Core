using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// JwtAuthService
    /// </summary>
    public static class JwtAuthServiceExtensions
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        public static IServiceCollection AddJwtAuthService(this IServiceCollection services)
        {
            // JWT Token颁发参数
            var audienceConfig = ComHelper.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var tokenValidationParameters = GetTokenValidationParameters(audienceConfig, signingKey);

            //2.1【认证】、core自带官方JWT认证
            // 开启Bearer认证
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = AuthHelper.JwtAuthScheme;
                o.DefaultChallengeScheme = nameof(ApiResponseHandler);
                o.DefaultForbidScheme = nameof(ApiResponseHandler);
            })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = tokenValidationParameters;
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = OnMessageReceived(),
                        OnTokenValidated = OnTokenValidated(),
                        OnChallenge = OnChallenge(),
                        OnForbidden = OnForbidden(),
                        OnAuthenticationFailed = OnAuthenticationFailed(),
                    };
                });

            return services;
        }

        /// <summary>
        /// Jwt颁发参数
        /// </summary>
        /// <param name="audienceConfig"></param>
        /// <param name="signingKey"></param>
        /// <returns></returns>
        private static TokenValidationParameters GetTokenValidationParameters(IConfigurationSection audienceConfig, SymmetricSecurityKey signingKey)
        {
            // 令牌验证参数
            var tokenParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),// 过期后延长
                RequireExpirationTime = true,
            };

            return tokenParams;
        }

        /// <summary>
        /// OnMessageReceived
        /// </summary>
        /// <returns></returns>
        private static Func<MessageReceivedContext, Task> OnMessageReceived()
        {
            return context =>
            {
                //// 屏蔽，现在Mvc跳转有Cookies认证，没必要保存Jwt Cookie
                //// 从Header获取token
                //var headers = context.Request.Headers;
                //if (headers.ContainsKey("Authorization") && headers["Authorization"].ToString().StartsWith(AuthHelper.JwtAuthScheme))
                //{
                //    return Task.CompletedTask;
                //}

                //// 从QueryString和Cookie获取token
                //var tokenFromOther = string.Empty;
                //string tokenFromQuery = context.Request.Query["jwtToken"];
                //string tokenFromCookie = context.Request.Cookies[AuthHelper.JwtAuthName];
                //tokenFromOther = !string.IsNullOrWhiteSpace(tokenFromOther) ? tokenFromQuery : !string.IsNullOrWhiteSpace(tokenFromCookie) ? tokenFromCookie : tokenFromOther;
                //if (!string.IsNullOrWhiteSpace(tokenFromOther))
                //{
                //    context.Request.Headers.Add("Authorization", $"{AuthHelper.JwtAuthScheme} {tokenFromOther}");
                //}

                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnTokenValidated
        /// </summary>
        /// <returns></returns>
        private static Func<TokenValidatedContext, Task> OnTokenValidated()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnChallenge
        /// </summary>
        /// <returns></returns>
        private static Func<JwtBearerChallengeContext, Task> OnChallenge()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnForbidden
        /// </summary>
        /// <returns></returns>
        private static Func<ForbiddenContext, Task> OnForbidden()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnAuthenticationFailed
        /// </summary>
        /// <returns></returns>
        private static Func<AuthenticationFailedContext, Task> OnAuthenticationFailed()
        {
            return context =>
            {
                // 如果过期，则把<是否过期>添加到，返回头信息中
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }

                return Task.CompletedTask;
            };
        }
    }

    /// <summary>
    /// API响应处理
    /// </summary>
    public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// 授权处理
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 401 处理
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(ComHelper.JsonSerialize(new ApiResponse(StatusCode.CODE401)));
        }

        /// <summary>
        /// 403 处理
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status403Forbidden;
            await Response.WriteAsync(ComHelper.JsonSerialize(new ApiResponse(StatusCode.CODE403)));
        }
    }

    /// <summary>
    /// API响应
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// 404状态吗
        /// </summary>
        public int Status { get; set; } = 404;

        /// <summary>
        /// 404提示
        /// </summary>
        public object Value { get; set; } = "No Found";

        /// <summary>
        /// API响应
        /// </summary>
        /// <param name="apiCode"></param>
        public ApiResponse(StatusCode apiCode)
        {
            switch (apiCode)
            {
                case StatusCode.CODE401:
                    {
                        Status = 401;
                        Value = "很抱歉，您无权访问该接口，请确保已经登录!";
                    }
                    break;

                case StatusCode.CODE403:
                    {
                        Status = 403;
                        Value = "很抱歉，您的访问权限等级不够，联系管理员!";
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Http状态
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// 401：未授权
        /// </summary>
        CODE401,

        /// <summary>
        /// 403：无权限
        /// </summary>
        CODE403,

        /// <summary>
        /// 404：不存在
        /// </summary>
        CODE404,

        /// <summary>
        /// 500：服务端错误
        /// </summary>
        CODE500
    }
}
