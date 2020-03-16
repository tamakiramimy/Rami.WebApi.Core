using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// AuthorizationService
    /// </summary>
    public static class AuthorizationService
    {
        /// <summary>
        /// AddAuthorizationService
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationService(this IServiceCollection services)
        {
            // 策略认证
            services.AddAuthorization(o =>
            {
                //o.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                //o.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                //o.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));
                //o.AddPolicy("A_S_O", policy => policy.RequireRole("Admin", "System", "Others"));

                // mvc策略
                o.AddPolicy(AuthConst.MvcAuthSimple, policy => policy.Requirements.Add(new MvcAuthorizeHandlerSimple()));

                var mvcAuthReq = new MvcAuthorizeHandlerReq() { LoginPath = "/Admin/Login" };
                o.AddPolicy(AuthConst.MvcAuthReq, policy => policy.Requirements.Add(mvcAuthReq));

                var mvcAuthReqAdmin = new MvcAuthorizeHandlerReq() { LoginPath = "/Admin/Login", Roles = new[] { EnumSysRoles.SuperAdmin } };
                o.AddPolicy(AuthConst.MvcAuthReqAdmin, policy => policy.Requirements.Add(mvcAuthReqAdmin));

                // api策略
                o.AddPolicy(AuthConst.ApiAuthSimple, policy => policy.Requirements.Add(new ApiAuthorizeHandlerSimple()));

                var apiAuthReq = new ApiAuthorizeHandlerReq() { };
                o.AddPolicy(AuthConst.ApiAuthReq, policy => policy.Requirements.Add(apiAuthReq));

                var apiAuthAdmin = new ApiAuthorizeHandlerReq()
                {
                    Roles = new EnumSysRoles[] { EnumSysRoles.SuperAdmin },
                    Issuer = ConfigHelper.JwtIssuer,
                    Audience = ConfigHelper.JwtAudience,
                };
                o.AddPolicy(AuthConst.ApiAuthReqAdmin, policy => policy.Requirements.Add(apiAuthAdmin));
            });

            // 注入权限处理器
            services.AddSingleton<IAuthorizationHandler, MvcAuthorizeHandler>();
            services.AddSingleton<IAuthorizationHandler, MvcAuthorizeHandlerSimple>();

            services.AddSingleton<IAuthorizationHandler, ApiAuthorizeHandler>();
            services.AddSingleton<IAuthorizationHandler, ApiAuthorizeHandlerSimple>();

            return services;
        }
    }
}
