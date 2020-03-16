using Rami.WebApi.Core.Framework;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// Memory缓存
    /// </summary>
    public static class MemoryCacheService
    {
        /// <summary>
        /// AddMemoryCacheService
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemoryCacheService(this IServiceCollection services)
        {
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            return services;
        }
    }
}
