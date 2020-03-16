using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 关注取关记录
    /// </summary>
    [SqlSugar.SugarTable("WxSubScribeLog")]
    public class WxSubScribeLog : BaseDbShow
    {
        /// <summary>
        /// 自增Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 0:关注;1:取消关注
        /// </summary>
        public int SubScribeType { get; set; }

        /// <summary>
        /// 关注/取关时间
        /// </summary>
        public DateTime OptDate { get; set; }

        /// <summary>
        /// 二维码场景key
        /// </summary>
        public string QrSceneStr { get; set; }

        /// <summary>
        /// 二维码Ticket
        /// </summary>
        public string QrTicketId { get; set; }
    }
}
