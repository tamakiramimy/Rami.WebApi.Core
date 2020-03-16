using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// appsettings.json操作类
    /// </summary>
    public class Appsettings
    {
        /// <summary>
        /// 配置
        /// </summary>
        internal static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="contentPath"></param>
        public Appsettings(string contentPath)
        {
            string Path = "appsettings.json";
            Configuration = new ConfigurationBuilder()
               .SetBasePath(contentPath)
               .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })//这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
               .Build();
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        private static string Key(params string[] sections)
        {
            try
            {
                var val = string.Empty;
                for (int i = 0; i < sections.Length; i++)
                {
                    val += sections[i] + ":";
                }

                return Configuration[val.TrimEnd(':')];
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static string Key(string sections)
        {
            try
            {
                return Configuration[sections];
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取配置对象
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static IConfigurationSection Section(string sections)
        {
            return Configuration.GetSection(sections);
        }
    }
}
