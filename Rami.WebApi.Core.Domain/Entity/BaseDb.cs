using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// BaseDb
    /// </summary>
    public class BaseDb
    {
        /// <summary>
        /// 是否禁用
        /// </summary>
        public int IsDel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string Updater { get; set; }
    }

    /// <summary>
    /// 数据库基类
    /// </summary>
    public class BaseDbShow : BaseDb
    {
        /// <summary>
        /// 是否禁用
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string IsDelShow
        {
            get
            {
                return IsDel == 0 ? "启用" : "禁用";
            }
        }
    }
}
