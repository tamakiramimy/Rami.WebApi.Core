using Rami.WebApi.Core.Framework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 全局异常日志
    /// </summary>
    public class GlobalExceptionsFilter : IExceptionFilter
    {
        /// <summary>
        /// 环境变量
        /// </summary>
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<GlobalExceptionsFilter> _loggerHelper;

        /// <summary>
        /// 异常通知Hub
        /// </summary>
        private readonly IHubContext<ChatHub> _hubContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="env"></param>
        /// <param name="loggerHelper"></param>
        /// <param name="hubContext"></param>
        public GlobalExceptionsFilter(IWebHostEnvironment env, ILogHelper<GlobalExceptionsFilter> loggerHelper, IHubContext<ChatHub> hubContext)
        {
            _env = env;
            _loggerHelper = loggerHelper;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var json = new JsonErrorResponse();
            json.Message = context.Exception.Message;//错误信息
            if (_env.IsDevelopment())
            {
                json.DevelopmentMessage = context.Exception.StackTrace;//堆栈信息
            }

            //context.Result = new InternalServerErrorObjectResult(json);
            // todo:全局异常改用Result返回
            var jsonResult = new Result { Code = "500", Message = ComHelper.JsonSerialize(json) };
            context.Result = new JsonResult(jsonResult) { StatusCode = StatusCodes.Status500InternalServerError };
            MiniProfiler.Current.CustomTiming("Errors：", json.Message);

            //采用log4net 进行错误日志记录
            var msg = WriteLog(json.Message, context.Exception);
            _loggerHelper.ExceptionLog(msg, context.Exception);
            //_hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData()).Wait();
        }

        /// <summary>
        /// 自定义返回格式
        /// </summary>
        /// <param name="throwMsg"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public string WriteLog(string throwMsg, Exception ex)
        {
            return string.Format("【自定义错误】：{0} \r\n【异常类型】：{1} \r\n【异常信息】：{2} \r\n【堆栈调用】：{3} \r\n",
                new object[] { throwMsg, ex.GetType().Name, ex.Message, ex.StackTrace });
        }
    }

    /// <summary>
    /// 系统内部错误
    /// </summary>
    public class InternalServerErrorObjectResult : ObjectResult
    {
        /// <summary>
        /// 系统内部错误
        /// </summary>
        /// <param name="value"></param>
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    /// <summary>
    /// 返回错误信息
    /// </summary>
    public class JsonErrorResponse
    {
        /// <summary>
        /// 生产环境的消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 开发环境的消息
        /// </summary>
        public string DevelopmentMessage { get; set; }
    }
}
