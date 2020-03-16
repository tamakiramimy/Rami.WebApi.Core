using Rami.WebApi.Core.Framework;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 日志拦截
    /// </summary>
    public class LogAOP : IInterceptor
    {
        /// <summary>
        /// 异常日志通知Hub
        /// </summary>
        private readonly IHubContext<ChatHub> _hubContext;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<LogAOP> _logHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="logHelper"></param>
        public LogAOP(IHubContext<ChatHub> hubContext, ILogHelper<LogAOP> logHelper)
        {
            _hubContext = hubContext;
            _logHelper = logHelper;
        }

        /// <summary>
        /// 实例化IInterceptor唯一方法 
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            //记录被拦截方法信息的日志信息
            var dataIntercept = "" +
                $"【当前执行方法】：{ invocation.Method.Name} \r\n" +
                $"【携带的参数有】： {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())} \r\n";

            try
            {
                MiniProfiler.Current.Step($"执行Service方法：{invocation.Method.Name}() -> ");
                //在被拦截的方法执行完毕后 继续执行当前方法，注意是被拦截的是异步的
                invocation.Proceed();

                // 异步获取异常，先执行
                if (IsAsyncMethod(invocation.Method))
                {
                    //Wait task execution and modify return value
                    if (invocation.Method.ReturnType == typeof(Task))
                    {
                        invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                            (Task)invocation.ReturnValue,
                            async () => await TestActionAsync(invocation),
                            ex =>
                            {
                                LogEx(ex, ref dataIntercept);
                            });
                    }
                    else
                    {
                        //Task<TResult>
                        invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                         invocation.Method.ReturnType.GenericTypeArguments[0],
                         invocation.ReturnValue,
                         async () => await TestActionAsync(invocation),
                         ex =>
                         {
                             LogEx(ex, ref dataIntercept);
                         });
                    }
                }
                else
                {
                    // 同步1
                }
            }
            catch (Exception ex)
            {
                // 同步2
                LogEx(ex, ref dataIntercept);
            }

            var type = invocation.Method.ReturnType;
            if (typeof(Task).IsAssignableFrom(type))
            {
                var resultProperty = type.GetProperty("Result");
                dataIntercept += ($"【执行完成结果】：{ComHelper.JsonSerialize(resultProperty?.GetValue(invocation.ReturnValue))}");
            }
            else
            {
                dataIntercept += ($"【执行完成结果】：{invocation.ReturnValue}");
            }

            Parallel.For(0, 1, e =>
            {
                //LogLock.OutSql2Log("AOPLog", new string[] { dataIntercept });
                _logHelper.AopLog($"LogAOP:执行日志:{dataIntercept}");
            });

            //_hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData()).Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        private async Task TestActionAsync(IInvocation invocation)
        {
            //Console.WriteLine("Waiting after method execution for " + invocation.MethodInvocationTarget.Name);
            await Task.Delay(20); // 仅作测试
            //Console.WriteLine("Waited after method execution for " + invocation.MethodInvocationTarget.Name);
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="dataIntercept"></param>
        private void LogEx(Exception ex, ref string dataIntercept)
        {
            if (ex != null)
            {
                //执行的 service 中，收录异常
                MiniProfiler.Current.CustomTiming("Errors：", ex.Message);
                //执行的 service 中，捕获异常
                dataIntercept += ($"方法执行中出现异常：{ex.Message + ex.InnerException}\r\n");
            }
        }

        /// <summary>
        /// 判断是否异步方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }
    }

    /// <summary>
    /// 异步方法拦截帮助类
    /// </summary>
    internal static class InternalAsyncHelper
    {
        /// <summary>
        /// 异步方法执行，插入Post自定义方法，并记录异常
        /// </summary>
        /// <param name="actualReturnValue"></param>
        /// <param name="postAction"></param>
        /// <param name="finalAction"></param>
        /// <returns></returns>
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
                await postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                finalAction(exception);
            }
        }

        /// <summary>
        /// 泛型异步方法执行，插入Post自定义方法，并记录异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actualReturnValue"></param>
        /// <param name="postAction"></param>
        /// <param name="finalAction"></param>
        /// <returns></returns>
        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                var result = await actualReturnValue;
                await postAction();
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        /// <summary>
        /// 异步泛型方法执行并返回，记录错误日志
        /// </summary>
        /// <param name="taskReturnType"></param>
        /// <param name="actualReturnValue"></param>
        /// <param name="action"></param>
        /// <param name="finalAction"></param>
        /// <returns></returns>
        public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType, object actualReturnValue, Func<Task> action, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper)
                .GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] { actualReturnValue, action, finalAction });
        }
    }
}
