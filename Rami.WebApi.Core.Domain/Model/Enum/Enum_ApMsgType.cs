using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 后台自动推送配置类别
    /// </summary>
    public enum Enum_ApMsgType
    {
        /// <summary>
        /// 文本
        /// </summary>
        [Description("文本")]
        Text = 1,
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        Image = 2,
        /// <summary>
        /// 录音
        /// </summary>
        [Description("录音")]
        Voice = 3,
        /// <summary>
        /// 公众号图文
        /// </summary>
        [Description("公众号图文")]
        News = 4,
        /// <summary>
        /// 视频
        /// </summary>
        [Description("视频")]
        Video = 5,
        /// <summary>
        /// 后台图文
        /// </summary>
        [Description("后台图文")]
        BackNews = 6,
        /// <summary>
        /// 红包
        /// </summary>
        [Description("红包")]
        RedBag = 7,
    }
}
