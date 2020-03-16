using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// MiniProfilerService
    /// </summary>
    public static class MiniProfilerService
    {
        /// <summary>
        /// AddMiniProfilerService
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMiniProfilerService(this IServiceCollection services)
        {
            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.Left;
                options.PopupShowTimeWithChildren = true;
            });

            return services;
        }
    }
}
