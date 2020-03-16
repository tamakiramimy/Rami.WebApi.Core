using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 数据库配置表
    /// </summary>
    [SqlSugar.SugarTable("DbConfig")]
    public class DbConfig : BaseDbShow
    {
        /// <summary>
        /// 配置名
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string ConfigName { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        public string ConfigValue { get; set; }

        /// <summary>
        /// 配置备注
        /// </summary>
        public string ConfigRemark { get; set; }

        /// <summary>
        /// 配置提示
        /// </summary>
        public string ConfigTag { get; set; }
    }
}
