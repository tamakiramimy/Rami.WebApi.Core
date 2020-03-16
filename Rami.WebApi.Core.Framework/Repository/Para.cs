using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 搜索条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Para<T>
    {
        /// <summary>
        /// 页数，默认第一页
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页多少条,默认10条
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Linq，对API接口无效
        /// </summary>
        public System.Linq.Expressions.Expression<Func<T, bool>> Filter { get; set; }

        /// <summary>
        /// 排序,对API接口无效
        /// </summary>
        public string OrderKey { get; set; }

        /// <summary>
        /// 最后时间戳,,为空时取全部
        /// </summary>
        public T Entity { get; set; }

        /// <summary>
        /// SQL语句
        /// </summary>
        public string SQL { get; set; }

        /// <summary>
        /// Where语句
        /// </summary>
        public string WhereSQL { get; set; }
    }

}
