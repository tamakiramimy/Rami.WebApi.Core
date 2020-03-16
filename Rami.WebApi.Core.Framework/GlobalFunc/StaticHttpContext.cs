using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 静态HttpContext
    /// </summary>
    public class StaticHttpContext
    {
        /// <summary>
        /// IHttpContextAccessor
        /// </summary>
        private static IHttpContextAccessor _accessor;

        /// <summary>
        /// 全局HttpContext
        /// </summary>
        public static HttpContext Current => _accessor.HttpContext;

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="accessor"></param>
        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }

    /// <summary>
    /// StaticHttpContext扩展
    /// </summary>
    public static class StaticHttpContextExtensions
    {
        ///// <summary>
        ///// IServiceCollection 注入 HttpContextAccessor
        ///// </summary>
        ///// <param name="services"></param>
        //public static void AddHttpContextAccessor(this IServiceCollection services)
        //{
        //    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        //}

        /// <summary>
        /// IApplicationBuilder 注入 httpContextAccessor
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            StaticHttpContext.Configure(httpContextAccessor);
            return app;
        }
    }
}
