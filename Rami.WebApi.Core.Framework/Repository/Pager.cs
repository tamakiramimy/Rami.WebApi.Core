using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 分页返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pager<T> : Result
    {
        /// <summary>
        /// 当前页标
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { set; get; } = 20;

        /// <summary>
        /// 总行数
        /// </summary>
        public Int64 TotalCount { get; set; } = 0;

        /// <summary>
        ///  总页数
        /// </summary>
        public Int64 TotalPages { get; set; } = 0;

        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Datas { get; set; }

        /// <summary>
        /// 自定义对象
        /// </summary>
        public object Custom { get; set; }
    }
}
