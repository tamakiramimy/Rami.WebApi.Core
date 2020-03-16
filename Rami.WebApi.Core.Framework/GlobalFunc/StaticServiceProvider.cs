using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 静态IServiceProvider
    /// </summary>
    public class StaticServiceProvider
    {
        /// <summary>
        /// 服务提供
        /// </summary>
        public static IServiceProvider Current;

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Configure(IServiceProvider serviceProvider)
        {
            Current = serviceProvider;
        }
    }

    /// <summary>
    /// 静态IServiceProvider
    /// </summary>
    public static class StaticServiceProviderExtensions
    {
        /// <summary>
        /// IApplicationBuilder 注入 IServiceProvider
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseStaticServiceProvider(this IApplicationBuilder app)
        {
            IServiceProvider applicationServices = app.ApplicationServices;
            StaticServiceProvider.Configure(applicationServices);
            return app;
        }
    }
}
