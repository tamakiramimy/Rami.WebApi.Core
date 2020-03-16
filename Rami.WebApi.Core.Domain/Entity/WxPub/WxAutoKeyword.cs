using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 公众号关键字回复
    /// </summary>
    [SqlSugar.SugarTable("WxAutoKeyword")]
    public class WxAutoKeyword : BaseDbShow
    {
        /// <summary>
        /// Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string AutoName { get; set; }

        /// <summary>
        /// 关键字(格式a、b、c)
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// 推送的消息类型（CommDomain.BusinessObject.WxNotify.ApMsgType）
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 文本内容
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string TextContent { get; set; }

        /// <summary>
        /// 微信媒体ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string MediaId { get; set; }

        /// <summary>
        /// 微信媒体名称
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string MediaName { get; set; }

        /// <summary>
        /// 微信视频缩略图媒体ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string VideoThumbMediaId { get; set; }

        /// <summary>
        /// 微信视频缩略图名称
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string VideoThumbMediaName { get; set; }

        /// <summary>
        /// 微信视频标题
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string VideoTitle { get; set; }

        /// <summary>
        /// 微信视频描述
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string VideoDescription { get; set; }

        /// <summary>
        /// 选中的后台文章IDS
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string ArtIds { get; set; }

        /// <summary>
        /// 多图文发送方式（0：随机一条，1：多图文)
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int SendType { get; set; }

        /// <summary>
        /// 红包金额
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int RedAmount { get; set; }

        /// <summary>
        /// 红包个数
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int RedCount { get; set; }

        /// <summary>
        /// 红包祝福语
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string RedWish { get; set; }

        /// <summary>
        /// 红包活动名
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string RedAct { get; set; }

        /// <summary>
        /// 红包备注
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string RedRemark { get; set; }
    }

    /// <summary>
    /// 公众号关键字回复
    /// </summary>
    public class WxAutoKeywordShow : WxAutoKeyword
    {
        /// <summary>
        /// 选中的后台图文列表
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<long> LstSelArtIds { get; set; }

        /// <summary>
        /// 预览的昵称
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string PrevNick { get; set; }

        /// <summary>
        /// 预览OpenId
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string OpenId { get; set; }
    }
}
