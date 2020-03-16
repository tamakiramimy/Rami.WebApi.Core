using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 自动回复记录
    /// </summary>
    [SqlSugar.SugarTable("WxAutoKeywordDetail")]
    public class WxAutoKeywordDetail : BaseDbShow
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 自动应答ID
        /// </summary>
        public long AutoId { get; set; }

        /// <summary>
        /// Opend
        /// </summary>
        public string Opend { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }
    }
}
