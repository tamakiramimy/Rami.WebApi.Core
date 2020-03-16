using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 微信用户额外信息
    /// </summary>
    [SqlSugar.SugarTable("WxUserInfoExt")]
    public class WxUserInfoExt : BaseDbShow
    {
        /// <summary>
        /// OpenId
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string OpenId { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Crm LeadId
        /// </summary>
        public int? LeadId { get; set; }

        /// <summary>
        /// Crm PersId
        /// </summary>
        public int? PersId { get; set; }
    }
}
