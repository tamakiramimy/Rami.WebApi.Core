using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 微信公众号用户交互记录
    /// </summary>
    [SqlSugar.SugarTable("WxUserOptLog")]
    public class WxUserOptLog : BaseDbShow
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public string MsgId { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息类型(枚举WeChat.Public.MsgType)
        /// </summary>
        public string MsgType { get; set; }

        /// <summary>
        /// 文本内容
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Content { get; set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string PicUrl { get; set; }

        /// <summary>
        /// 媒体ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string MediaId { get; set; }

        /// <summary>
        /// 媒体格式
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Format { get; set; }

        /// <summary>
        /// 缩略图媒体ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string ThumbMediaId { get; set; }

        /// <summary>
        /// 坐标维度
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string LocationX_Latitude { get; set; }

        /// <summary>
        /// 坐标经度
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string LocationY_Longitude { get; set; }

        /// <summary>
        /// 地图缩放级别
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Scale { get; set; }

        /// <summary>
        /// 坐标地点名称
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string LabelPrecision { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Title { get; set; }

        /// <summary>
        /// 文章描述
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 文章链接
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Url { get; set; }

        /// <summary>
        /// 事件类别(枚举WeChat.Public.EventType)
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Event { get; set; }

        /// <summary>
        /// 事件Key
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string EventKey { get; set; }

        /// <summary>
        /// 卡券Ticket?
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Ticket { get; set; }
    }
}
