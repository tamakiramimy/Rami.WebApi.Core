using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 后台图文（自动推送用）
    /// </summary>
    [SqlSugar.SugarTable("WxBackNews")]
    public class WxBackNews : BaseDbShow
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 图文名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图文描述
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string SecondName { get; set; }

        /// <summary>
        /// 文章内容（html）
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string HtmlContent { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int VistorNumber { get; set; }

        /// <summary>
        /// 文章链接
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string ArticleUrl { get; set; }

        /// <summary>
        /// 图片地址(Upload/Files)
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string ImgUrlVir { get; set; }
    }

    /// <summary>
    /// GzhMarket后台图文（自动推送用）
    /// </summary>
    public class WxBackNewsShow : WxBackNews
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string ImgUrlShow { get; set; }

        /// <summary>
        /// 图文链接
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string ArticleUrlShow { get; set; }

        /// <summary>
        /// 上传的文件列表
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public UploadFileInfo UpFile { get; set; }
    }
}
