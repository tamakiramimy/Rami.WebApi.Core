using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 上传文件路径帮助类
    /// </summary>
    public class UpPathHelper
    {
        /// <summary>
        /// 上传目录定义
        /// </summary>
        public static Dictionary<string, string> DicUpPath = new Dictionary<string, string>
        {
            // 默认路径
            {"files" ,"/Upload/Files/"},
            // 视频
            {"videos","/Upload/Videos/" },
            // 微信永久素材 WxMedia
            {"wxmedia","/Upload/WxMedia/" },
        };

        /// <summary>
        /// 根据上传目录字典获取物理上传路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetUploadPhyFullPath(string key)
        {
            return GetUploadPhyPath(key, DateTime.Now);
        }

        /// <summary>
        /// 根据上传目录字典获取物理上传路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetUploadPhyPath(string key, DateTime dt)
        {
            key = key.ToLower();
            if (DicUpPath.ContainsKey(key))
            {
                var virPath = DicUpPath[key];
                var path = ComHelper.GetPhyWWWRoot(virPath);
                var fullPath = Path.Combine(path, dt.ToString("yyyyMMdd"));
                return fullPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// 根据上传目录字典获取绝对上传路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetUploadAbsFullPath(string key)
        {
            return GetUploadPhyPath(key, DateTime.Now);
        }

        /// <summary>
        /// 根据上传目录字典获取绝对路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetUploadAbsPath(string key, DateTime dt)
        {
            key = key.ToLower();
            if (DicUpPath.ContainsKey(key))
            {
                var virPath = DicUpPath[key];
                var path = ComHelper.GetAbsPath(virPath);
                var fullPath = Path.Combine(path, dt.ToString("yyyyMMdd"));
                return fullPath;
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据上传目录字典获取虚拟上传路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetUploadVirFullPath(string key)
        {
            return GetUploadPhyPath(key, DateTime.Now);
        }

        /// <summary>
        /// 根据上传目录字典获取虚拟路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetUploadVirPath(string key, DateTime dt)
        {
            key = key.ToLower();
            if (DicUpPath.ContainsKey(key))
            {
                var virPath = DicUpPath[key];
                var fullPath = Path.Combine(virPath, dt.ToString("yyyyMMdd"));
                return fullPath;
            }

            return string.Empty;
        }
    }
}
