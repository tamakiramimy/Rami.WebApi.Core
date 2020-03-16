using Rami.WebApi.Core.Framework;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 中间件
    /// 记录请求和响应数据
    /// </summary>
    public class RequRespLogMildd
    {
        /// <summary>
        /// 下一个请求
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<RequRespLogMildd> _logHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logHelper"></param>
        public RequRespLogMildd(RequestDelegate next, ILogHelper<RequRespLogMildd> logHelper)
        {
            _next = next;
            _logHelper = logHelper;
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // 过滤，只有接口
            if (context.Request.Path.Value.Contains("api"))
            {
                //context.Request.EnableRewind();
                context.Request.EnableBuffering();
                Stream originalBody = context.Response.Body;

                try
                {
                    // 存储请求数据
                    var reqMsg = await RequestDataLog(context.Request);
                    using (var ms = new MemoryStream())
                    {
                        context.Response.Body = ms;
                        await _next(context);

                        // 存储响应数据
                        var respMsg = await ResponseDataLog(context.Response, ms);
                        _logHelper.ApiLog($"RequRespLogMildd:{reqMsg}{respMsg}");

                        ms.Position = 0;
                        await ms.CopyToAsync(originalBody);
                    }
                }
                catch (Exception ex)
                {
                    // 记录异常
                    _logHelper.Error($"RequRespLogMildd:发生异常：{ex.Message}", ex);
                }
                finally
                {
                    context.Response.Body = originalBody;
                }
            }
            else
            {
                await _next(context);
            }
        }

        /// <summary>
        /// 请求日志
        /// </summary>
        /// <param name="request"></param>
        private async Task<string> RequestDataLog(HttpRequest request)
        {
            var reader = new StreamReader(request.Body);
            var reqData = await reader.ReadToEndAsync();
            var msg = $"请求参数:Path:{request.Path}    Query:{ComHelper.JsonSerialize(request.Query)}    Body:{reqData} \r\n";
            request.Body.Position = 0;
            return msg;
        }

        /// <summary>
        /// 响应日志
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ms"></param>
        private async Task<string> ResponseDataLog(HttpResponse response, MemoryStream ms)
        {
            ms.Position = 0;
            var reader = new StreamReader(ms);
            var respData = await reader.ReadToEndAsync();
            var msg = $"返回结果:{respData} \r\n";
            return msg;
        }
    }
}
