using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 推送记录/红包发放记录
    /// </summary>
    [SqlSugar.SugarTable("WxAutoPushDetail")]
    public class WxAutoPushDetail : BaseDbShow
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 推送配置ID
        /// </summary>
        public long PushId { get; set; }

        /// <summary>
        /// Opend
        /// </summary>
        public string Opend { get; set; }

        /// <summary>
        /// 推送时间
        /// </summary>
        public DateTime PushTime { get; set; }
    }
}
