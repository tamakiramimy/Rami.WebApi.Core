using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rami.WebApi.Core.Service.Helpers
{
    /// <summary>
    /// 文件上传帮助类
    /// </summary>
    public class UploadFileHelper
    {
        /// <summary>
        /// 默认根目录
        /// </summary>
        private static readonly string Root = ComHelper.WWWRoot;

        /// <summary>
        /// 根据相对路径获取上传文件信息
        /// </summary>
        /// <param name="virPath"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static UploadFileInfo GetUploadFileInfo(string virPath, string root = null)
        {
            if (string.IsNullOrWhiteSpace(root))
            {
                root = Root;
            }

            // 文件信息
            var file = new UploadRes { VirPath = virPath };
            file.OrgFileName = Path.GetFileName(virPath);
            file.PhyPath = ComHelper.GetPhyPath(root, virPath);
            file.AbsPath = ComHelper.GetAbsPath(virPath);

            return new UploadFileInfo
            {
                name = file.OrgFileName,
                response = new UploadFileResp
                {
                    IsSucc = true,
                    Data = new List<UploadRes> { file }
                }
            };
        }

        /// <summary>
        /// 根据相对路径获取批量上传文件信息
        /// </summary>
        /// <param name="lstVirPath"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static List<UploadFileInfo> GetUploadFileInfos(List<string> lstVirPath, string root = null)
        {
            var lstRes = new List<UploadFileInfo> { };
            foreach (var virPath in lstVirPath)
            {
                var upRes = GetUploadFileInfo(virPath, root);
                lstRes.Add(upRes);
            }

            return lstRes;
        }
    }
}
