using Microsoft.AspNetCore.Builder;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 中间件帮助类
    /// </summary>
    public static class MiddlewareHelpers
    {
        /// <summary>
        /// Jwt验证
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtTokenAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtTokenAuth>();
        }

        /// <summary>
        /// API请求响应日志
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseReuestResponseLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequRespLogMildd>();
        }
    }
}
