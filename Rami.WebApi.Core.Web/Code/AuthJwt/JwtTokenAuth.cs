using Rami.WebApi.Core.Framework;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 中间件
    /// 原做为自定义授权中间件
    /// 现做检查 header token的使用
    /// </summary>
    public class JwtTokenAuth
    {
        /// <summary>
        /// 下一请求
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<JwtTokenAuth> _logHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logHelper"></param>
        public JwtTokenAuth(RequestDelegate next, ILogHelper<JwtTokenAuth> logHelper)
        {
            _next = next;
            _logHelper = logHelper;
        }

        /// <summary>
        /// 请求预处理
        /// </summary>
        /// <param name="next"></param>
        private void PreProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke preproceed");
            //...
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="next"></param>
        private void PostProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke postproceed");
            //....
        }

        /// <summary>
        /// 验证Jwt授权
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            PreProceed(httpContext);

            //检测是否包含'Authorization'请求头
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                PostProceed(httpContext);
                return _next(httpContext);
            }

            var tokenHeader = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                if (tokenHeader.Length >= 128)
                {
                    TokenModelJwt tm = AuthHelper.DecryptJwtToken(tokenHeader);

                    //授权
                    //var claimList = new List<Claim>();
                    //var claim = new Claim(ClaimTypes.Role, tm.Role);
                    //claimList.Add(claim);
                    //var identity = new ClaimsIdentity(claimList);
                    //var principal = new ClaimsPrincipal(identity);
                    //httpContext.User = principal;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"{DateTime.Now} middleware wrong:{e.Message}");
                _logHelper.Error($"JwtTokenAuth:出现异常：{ex.Message}", ex);
            }

            PostProceed(httpContext);
            return _next(httpContext);
        }
    }
}
