using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// CookiesAuthService
    /// </summary>
    public static class CookiesAuthService
    {
        /// <summary>
        /// AddAuthorizationService
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCookiesAuthService(this IServiceCollection services)
        {
            services.AddAuthentication(AuthHelper.CookieAuthScheme)
              .AddCookie(o =>
              {
                  //o.LoginPath = new PathString("/Admin/Login");
                  o.Cookie = new CookieBuilder { HttpOnly = true, Name = AuthHelper.CookieAuthName };
                  o.ExpireTimeSpan = TimeSpan.FromHours(ConfigHelper.JwtOverSec);
                  o.Events = new CookieAuthenticationEvents
                  {
                      OnRedirectToAccessDenied = OnRedirectToAccessDenied(),
                      OnRedirectToLogin = OnRedirectToLogin(),
                      OnRedirectToLogout = OnRedirectToLogout(),
                      OnRedirectToReturnUrl = OnRedirectToReturnUrl(),
                      OnSignedIn = OnSignedIn(),
                      OnSigningIn = OnSigningIn(),
                      OnSigningOut = OnSigningOut(),
                      OnValidatePrincipal = OnValidatePrincipal(),
                  };
              });

            return services;
        }

        /// <summary>
        /// OnValidatePrincipal
        /// </summary>
        /// <returns></returns>
        private static Func<CookieValidatePrincipalContext, Task> OnValidatePrincipal()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnSigningOut
        /// </summary>
        /// <returns></returns>
        private static Func<CookieSigningOutContext, Task> OnSigningOut()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnSigningIn
        /// </summary>
        /// <returns></returns>
        private static Func<CookieSigningInContext, Task> OnSigningIn()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnSignedIn
        /// </summary>
        /// <returns></returns>
        private static Func<CookieSignedInContext, Task> OnSignedIn()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnRedirectToLogout
        /// </summary>
        /// <returns></returns>
        private static Func<RedirectContext<CookieAuthenticationOptions>, Task> OnRedirectToLogout()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// RedirectContext
        /// </summary>
        /// <returns></returns>
        private static Func<RedirectContext<CookieAuthenticationOptions>, Task> OnRedirectToReturnUrl()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnRedirectToAccessDenied
        /// </summary>
        /// <returns></returns>
        private static Func<RedirectContext<CookieAuthenticationOptions>, Task> OnRedirectToAccessDenied()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// OnRedirectToLogin
        /// </summary>
        /// <returns></returns>
        private static Func<RedirectContext<CookieAuthenticationOptions>, Task> OnRedirectToLogin()
        {
            return context =>
            {
                return Task.CompletedTask;
            };
        }
    }

    /// <summary>
    /// 自定义
    /// </summary>
    public class CustomCookieAuthEvent : CookieAuthenticationEvents
    {
        /// <summary>
        /// RedirectToAccessDenied
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            return base.RedirectToAccessDenied(context);
        }

        /// <summary>
        /// RedirectToLogin
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            return base.RedirectToLogin(context);
        }

        /// <summary>
        /// RedirectToLogout
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task RedirectToLogout(RedirectContext<CookieAuthenticationOptions> context)
        {
            return base.RedirectToLogout(context);
        }

        /// <summary>
        /// RedirectToReturnUrl
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task RedirectToReturnUrl(RedirectContext<CookieAuthenticationOptions> context)
        {
            return base.RedirectToReturnUrl(context);
        }

        /// <summary>
        /// SignedIn
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task SignedIn(CookieSignedInContext context)
        {
            return base.SignedIn(context);
        }

        /// <summary>
        /// SigningIn
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task SigningIn(CookieSigningInContext context)
        {
            return base.SigningIn(context);
        }

        /// <summary>
        /// SigningOut
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task SigningOut(CookieSigningOutContext context)
        {
            return base.SigningOut(context);
        }

        /// <summary>
        /// ValidatePrincipal
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            return base.ValidatePrincipal(context);
        }
    }
}
