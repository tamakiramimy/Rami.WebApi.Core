using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 微信永久素材管理
    /// </summary>
    [SqlSugar.SugarTable("WxNews")]
    public class WxNews : BaseDbShow
    {
        /// <summary>
        /// 媒体ID
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string MediaId { get; set; }

        /// <summary>
        /// 首图文标题
        /// </summary>
        public string FirstNewsTitle { get; set; }

        /// <summary>
        /// 是否多图文
        /// </summary>
        public int IsMultiple { get; set; }
    }

    /// <summary>
    /// 微信永久素材管理
    /// </summary>
    public class WxNewsShow : WxNews
    {
        /// <summary>
        /// 是否多图文
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string IsMultipleShow
        {
            get
            {
                return IsMultiple == 1 ? "多图文" : "单图文";
            }
        }
    }
}
