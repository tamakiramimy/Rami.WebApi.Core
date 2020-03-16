using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// Id4 Oidc认证
    /// </summary>
    public static class Id4OidcService
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddId4OidcService(this IServiceCollection services)
        {
            var config = ComHelper.GetConf("AppSettings:OidcUrlRoot");
            // HybridAndClientCredentials 方式要求客户端和服务端都必须是Https 带SSL 端口
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = AuthHelper.CookieAuthScheme;
                options.DefaultChallengeScheme = AuthHelper.Id4OidcScheme;
            })
                .AddCookie(AuthHelper.CookieAuthScheme, o =>
                {
                    o.Cookie = new CookieBuilder { HttpOnly = true, Name = AuthHelper.CookieAuthName };
                })
                .AddOpenIdConnect(AuthHelper.Id4OidcScheme, options =>
                {
                    options.SignInScheme = AuthHelper.CookieAuthScheme;

                    options.Authority = config;
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "Rami.WebApi.Core.Web";
                    options.ClientSecret = ConfigHelper.JwtSecret;
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("webapi");
                    options.Scope.Add("offline_access");
                    options.CorrelationCookie = new CookieBuilder { HttpOnly = true, Name = "Id4Oidc" };
                    options.Events = new OpenIdConnectEvents
                    {
                        OnAuthenticationFailed = OnAuthenticationFailed(),
                        OnAuthorizationCodeReceived = OnAuthorizationCodeReceived(),
                        OnMessageReceived = OnMessageReceived(),
                        OnRedirectToIdentityProvider = OnRedirectToIdentityProvider(),
                        OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut(),
                        OnSignedOutCallbackRedirect = OnSignedOutCallbackRedirect(),
                        OnRemoteSignOut = OnRemoteSignOut(),
                        OnTokenResponseReceived = OnTokenResponseReceived(),
                        OnTokenValidated = OnTokenValidated(),
                        OnUserInformationReceived = OnUserInformationReceived()
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                    };
                });

            IdentityModelEventSource.ShowPII = true;

            return services;
        }

        /// <summary>
        /// OnAuthenticationFailed
        /// </summary>
        /// <returns></returns>
        private static Func<AuthenticationFailedContext, Task> OnAuthenticationFailed()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnAuthorizationCodeReceived
        /// </summary>
        /// <returns></returns>
        private static Func<AuthorizationCodeReceivedContext, Task> OnAuthorizationCodeReceived()
        {
            return context =>
            {
                //// 构造JwtToken
                //var tokenModel = new TokenModelJwt();
                //tokenModel.UserName = 

                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnMessageReceived
        /// </summary>
        /// <returns></returns>
        private static Func<MessageReceivedContext, Task> OnMessageReceived()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnRedirectToIdentityProvider
        /// </summary>
        /// <returns></returns>
        private static Func<RedirectContext, Task> OnRedirectToIdentityProvider()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnRedirectToIdentityProviderForSignOut
        /// </summary>
        /// <returns></returns>
        private static Func<RedirectContext, Task> OnRedirectToIdentityProviderForSignOut()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnSignedOutCallbackRedirect
        /// </summary>
        /// <returns></returns>
        private static Func<RemoteSignOutContext, Task> OnSignedOutCallbackRedirect()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnRemoteSignOut
        /// </summary>
        /// <returns></returns>
        private static Func<RemoteSignOutContext, Task> OnRemoteSignOut()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnTokenResponseReceived
        /// </summary>
        /// <returns></returns>
        private static Func<TokenResponseReceivedContext, Task> OnTokenResponseReceived()
        {
            return context =>
            {
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
        /// OnUserInformationReceived
        /// </summary>
        /// <returns></returns>
        private static Func<UserInformationReceivedContext, Task> OnUserInformationReceived()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }
    }
}
