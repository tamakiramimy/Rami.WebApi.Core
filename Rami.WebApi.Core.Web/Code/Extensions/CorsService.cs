using Rami.WebApi.Core.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// Cors
    /// </summary>
    public static class CorsService
    {
        /// <summary>
        /// AddAuthorizationService
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCorsService(this IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddPolicy("LimitRequests", policy =>
                {
                    // 支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
                    // 注意，http://127.0.0.1:1818 和 http://localhost:1818 是不一样的，尽量写两个
                    policy
                    .WithOrigins(ConfigHelper.AllowOrigins)
                    .AllowAnyHeader()//Ensures that the policy allows any header.
                    .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
