using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 微信图文详情
    /// </summary>
    [SqlSugar.SugarTable("WxNewsDetail")]
    public class WxNewsDetail : BaseDbShow
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 媒体ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string MediaId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Title { get; set; }

        /// <summary>
        /// 缩略图ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string ThumbMediaId { get; set; }

        /// <summary>
        /// 微信端图片（同步图文消息时返回）
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string ThumbUrl { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Author { get; set; }

        /// <summary>
        /// 图文摘要
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Digest { get; set; }

        /// <summary>
        /// 是否显示封面图
        /// </summary>
        public int ShowCoverPic { get; set; }

        /// <summary>
        /// 图文Html内容
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Content { get; set; }

        /// <summary>
        /// 图文链接
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string ContentSourceUrl { get; set; }

        /// <summary>
        /// 微信图文链接（同步图文消息时返回）
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string WxUrl { get; set; }
    }

    /// <summary>
    /// 微信图文详情
    /// </summary>
    public class WxNewsDetailShow : WxNewsDetail
    {
        /// <summary>
        /// 图文链接
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string ContentSourceUrlShow { get; set; }

        /// <summary>
        /// 缩略图展示
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string ThumbUrlShow { get; set; }
    }
}
