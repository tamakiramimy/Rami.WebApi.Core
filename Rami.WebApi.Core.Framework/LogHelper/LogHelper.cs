using log4net;
using System;
using System.Collections.Concurrent;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelper<T> : ILogHelper<T> where T : class
    {
        /// <summary>
        /// Class和日志对象字典
        /// </summary>
        private readonly ConcurrentDictionary<Type, ILog> Loggers = new ConcurrentDictionary<Type, ILog>();

        /// <summary>
        /// 日志创库
        /// </summary>
        private static string repository = ComHelper.GetConf("Logging:Log4Net:Name");

        /// <summary>
        /// 普通日志
        /// </summary>
        private readonly ILog comLog = LogManager.GetLogger(repository, "ComLog");

        /// <summary>
        /// ApiLog
        /// </summary>
        private readonly ILog apiLog = LogManager.GetLogger(repository, "ApiLog");

        /// <summary>
        /// SqlLog
        /// </summary>
        private readonly ILog sqlLog = LogManager.GetLogger(repository, "SqlLog");

        /// <summary>
        /// ExceptionLog
        /// </summary>
        private readonly ILog exceptionLog = LogManager.GetLogger(repository, "ExceptionLog");

        /// <summary>
        /// AopLog
        /// </summary>
        private readonly ILog aopLog = LogManager.GetLogger(repository, "AopLog");

        #region 普通日志

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Debug(object message, Exception exception = null)
        {
            comLog.Debug(message, exception);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(object message, Exception exception = null)
        {
            ExceptionLog(message, exception);
        }

        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Fatal(object message, Exception exception = null)
        {
            comLog.Fatal(message, exception);
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Info(object message, Exception exception = null)
        {
            comLog.Info(message, exception);
        }

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Warn(object message, Exception exception = null)
        {
            comLog.Warn(message, exception);
        }

        #endregion

        /// <summary>
        /// ApiLog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void ApiLog(object message, Exception exception = null)
        {
            apiLog.Debug(message, exception);
        }

        /// <summary>
        /// SqlLog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void SqlLog(object message, Exception exception = null)
        {
            sqlLog.Debug(message, exception);
        }

        /// <summary>
        /// ExceptionLog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void ExceptionLog(object message, Exception exception = null)
        {
            exceptionLog.Debug(message, exception);
        }

        /// <summary>
        /// AopLog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void AopLog(object message, Exception exception = null)
        {
            aopLog.Debug(message, exception);
        }
    }
}
