using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 公众号微信扫码记录
    /// </summary>
    [SqlSugar.SugarTable("WxQrCodeLog")]
    public class WxQrCodeLog : BaseDbShow
    {
        /// <summary>
        /// Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 扫码时间
        /// </summary>
        public DateTime ScanDate { get; set; }

        /// <summary>
        /// 二维码场景key
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string QrSceneStr { get; set; }

        /// <summary>
        /// 二维码Ticket
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string QrTicketId { get; set; }

        /// <summary>
        /// 二维码关联字段（推广员Id等）
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string QrStr { get; set; }
    }
}
