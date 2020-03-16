using Rami.WebApi.Core.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 文件上传结果
    /// </summary>
    public class UploadRes
    {
        /// <summary>
        /// 上传时的文件名
        /// </summary>
        public string OrgFileName { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        public string VirPath { get; set; }

        /// <summary>
        /// 绝对路径
        /// </summary>
        public string AbsPath { get; set; }

        /// <summary>
        /// 物理路径
        /// </summary>
        public string PhyPath { get; set; }
    }

    /// <summary>
    /// 上传结果
    /// </summary>
    public class UploadFileInfo
    {
        /// <summary>
        /// 上传状态
        /// </summary>
        public string status { get; set; } = "success";

        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 上传文件后台响应结果
        /// </summary>
        public UploadFileResp response { get; set; }
    }

    /// <summary>
    /// 上传文件后台响应结果
    /// </summary>
    public class UploadFileResp : Result<List<UploadRes>>
    {
    }
}
