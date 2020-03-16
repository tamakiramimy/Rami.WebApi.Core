using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 自动推送配置
    /// </summary>
    [SqlSugar.SugarTable("WxAutoPush")]
    public class WxAutoPush : BaseDbShow
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string PushName { get; set; }

        /// <summary>
        /// 推送的消息类型（Rami.WebApi.Core.Domain.ApMsgType）
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 设定推送时间
        /// </summary>
        public DateTime PushTime { get; set; }

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
        /// 是否多图文
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int IsMul { get; set; }

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

        /// <summary>
        /// 按性别发送
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string PushingSex { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Country { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string City { get; set; }

        /// <summary>
        /// 按曾经发送过关键字的用户发送
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string OldKeyWords { get; set; }
    }

    /// <summary>
    /// 自动推送配置
    /// </summary>
    public class WxAutoPushShow : WxAutoPush
    {
        /// <summary>
        /// 推送人数
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public int PushCount { get; set; }

        /// <summary>
        /// 是否能够修改
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public bool IsCanEdit
        {
            get
            {
                return DateTime.Now <= PushTime;
            }
        }

        /// <summary>
        /// 选中的性别
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<string> LstSelSex { get; set; }

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
        /// OpenID
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string OpenID { get; set; }
    }
}
