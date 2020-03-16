using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 微信永久素材管理
    /// </summary>
    [SqlSugar.SugarTable("WxMedia")]
    public class WxMedia : BaseDbShow
    {
        /// <summary>
        /// 媒体ID
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string MediaId { get; set; }

        /// <summary>
        /// 素材类别
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 素材的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 素材的描述	
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Introduction { get; set; }

        /// <summary>
        /// 素材链接(微信服务器)
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true,Length = 500)]
        public string WxUrl { get; set; }

        /// <summary>
        /// 本地存储路径
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string LocalUrl { get; set; }
    }

    /// <summary>
    /// 微信永久素材管理
    /// </summary>
    public class WxMediaShow : WxMedia
    {
        /// <summary>
        /// 媒体类别
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string TypeShow { get; set; }

        /// <summary>
        /// 本地存储路径显示
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string LocalUrlShow { get; set; }
    }
}
