using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 结果
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 错误码
        /// </summary>
        private string _code = "0000";

        /// <summary>
        /// 状态码：0000为成功 0001验证失败
        /// </summary>
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSucc { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }
}
