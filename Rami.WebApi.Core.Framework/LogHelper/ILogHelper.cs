using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogHelper
    {
        #region 普通日志

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Debug(object message, Exception exception = null);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(object message, Exception exception = null);

        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Fatal(object message, Exception exception = null);

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Info(object message, Exception exception = null);

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Warn(object message, Exception exception = null);

        #endregion

        /// <summary>
        /// ApiLog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void ApiLog(object message, Exception exception = null);

        /// <summary>
        /// SqlLog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void SqlLog(object message, Exception exception = null);

        /// <summary>
        /// ExceptionLog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void ExceptionLog(object message, Exception exception = null);

        /// <summary>
        /// AopLog
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void AopLog(object message, Exception exception = null);
    }

    /// <summary>
    /// 日志接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogHelper<T> : ILogHelper where T : class
    {
    }
}
