using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.PlatformAbstractions;
using System.Runtime.InteropServices;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 通用帮助类
    /// </summary>
    public static class ComHelper
    {
        #region 序列化

        /// <summary>
        /// 将对象转换成json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });
        }

        /// <summary>
        /// 将对象转换成json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerializeNoNull(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });
        }

        /// <summary>
        /// Json反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 字节序列化(redis)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] ByteSerialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        /// <summary>
        /// 字节反序列化(redis)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ByteDeserialize<T>(byte[] value)
        {
            if (value == null)
            {
                return default(T);
            }

            var jsonString = Encoding.UTF8.GetString(value);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// 将对象转换成xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string XmlSerializer<T>(T t)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            xmlSerializer.Serialize(writer, t);
            writer.Close();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Xml反序列化  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                return (T)xmldes.Deserialize(sr);
            }
        }

        /// <summary>
        /// Xml反序列化  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(T));
            return (T)xmldes.Deserialize(stream);
        }

        #endregion

        #region 配置文件

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="key"></param>
        public static string GetConf(string key)
        {
            return Appsettings.Key(key);
        }

        /// <summary>
        /// 获取配置对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationSection GetSection(string key)
        {
            return Appsettings.Section(key);
        }

        #endregion

        #region HTML处理

        /// <summary>
        /// 去除HTML代码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StripHT(this string text)
        {
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            return regex.Replace(text, "");
        }

        /// <summary>
        /// 去掉空字符串,用于微信接受对象
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string WxTargetFormat(string target)
        {
            return string.IsNullOrEmpty(target) ? string.Empty : string.Join("|", target.Split('|').Where(t => !string.IsNullOrEmpty(t)));
        }

        /// <summary>
        /// 去除富文本中的HTML标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = Regex.Replace(html, "<[^>]+>", "");
            strText = Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }

        #endregion

        #region 路径转换

        /// <summary>
        /// 环境变量
        /// </summary>
        /// <returns></returns>
        public static IHostingEnvironment GetHostingEnvironment()
        {
            return StaticServiceProvider.Current.GetRequiredService<IHostingEnvironment>();
        }

        /// <summary>
        /// WWWRoot文件夹物理路径
        /// D:\Projects\Svn\开发二组\201.市场品牌部Core\出国考试公众号\1.后端\Rami.WebApi.Core.Web\wwwroot
        /// </summary>
        public static string WWWRoot => GetHostingEnvironment().WebRootPath;

        /// <summary>
        /// 项目根目录物理路径
        /// D:\Projects\Svn\开发二组\201.市场品牌部Core\出国考试公众号\1.后端\Rami.WebApi.Core.Web
        /// </summary>
        public static string ContentRoot => GetHostingEnvironment().ContentRootPath;

        /// <summary>
        /// 当前运行环境（Development、Publish）
        /// Development 
        /// </summary>
        public static string EnvName => GetHostingEnvironment().EnvironmentName;

        /// <summary>
        /// AppDomain
        /// D:\Projects\Svn\开发二组\201.市场品牌部Core\出国考试公众号\1.后端\Rami.WebApi.Core.Web\bin\Debug\netcoreapp3.1\
        /// NETCoreApp,Version=v3.1
        /// </summary>
        public static AppDomain AppDomain => AppDomain.CurrentDomain;

        /// <summary>
        /// 当前上下文
        /// </summary>
        public static HttpContext HttpContext => StaticHttpContext.Current;

        #region 绝对路径

        /// <summary>
        /// 获取绝对路径
        /// https://www.baidu.com
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string GetAbsPath(string strUrl)
        {
            if (!string.IsNullOrEmpty(strUrl))
            {
                // 根目录Url
                var req = HttpContext.Request;
                var root = $"{req.Scheme}://{req.Host}";
                var newUrl = $"{root}/{strUrl}".ReplaceAbsPath();
                return newUrl;
            }

            return string.Empty;
        }

        /// <summary>
        /// 更新图片的绝对路径
        /// </summary>
        /// <param name="root"></param>
        /// <param name="virPath"></param>
        /// <returns></returns>
        public static string UpdImgAbsPath(string root, string virPath)
        {
            if (!string.IsNullOrEmpty(root) && !string.IsNullOrEmpty(virPath))
            {
                return $"{root}{virPath.ReplaceAbsPath()}";
            }

            return virPath;
        }

        /// <summary>
        /// 绝对路径格式化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceAbsPath(this string str)
        {
            return str.Replace("~", string.Empty).Replace("\\", "/").Replace("//", "/")
                .Replace("http:/", "http://").Replace("https:/", "https://");
        }

        #endregion

        #region 物理路径

        /// <summary>
        /// 获取物理路径
        /// D:\Projects\Svn\开发二组\201.市场品牌部Core\出国考试公众号\1.后端\Rami.WebApi.Core.Web\wwwroot
        /// </summary>
        /// <param name="root"></param>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string GetPhyPath(string root, string strUrl)
        {
            if (!string.IsNullOrEmpty(strUrl))
            {
                var urll = $"{root}\\{strUrl}".ReplacePhyPath();
                return urll;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取文件物理路径 相对于根目录）
        /// .NetCore上传文件只能放wwwroot文件夹里面，否则404无法浏览
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string GetPhyContentRoot(string strUrl)
        {
            //var hostEnv = GetHostingEnvironment().ContentRootFileProvider;
            //var dirInfo = hostEnv.GetDirectoryContents(strUrl);
            //var fileInfo = hostEnv.GetFileInfo(strUrl);
            return GetPhyPath(ContentRoot, strUrl);
        }

        /// <summary>
        /// 获取WWWRoot文件夹 文件物理路径
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string GetPhyWWWRoot(string strUrl)
        {
            //var hostEnv = GetHostingEnvironment().WebRootFileProvider;
            //var dirInfo = hostEnv.GetDirectoryContents(strUrl);
            //var fileInfo = hostEnv.GetFileInfo(strUrl);
            return GetPhyPath(WWWRoot, strUrl);
        }

        /// <summary>
        /// 物理路径格式化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplacePhyPath(this string str)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // windows物理路径
                return str.Replace("~", string.Empty).Replace("/", "\\").Replace("\\\\", "\\");
            }
            else
            { 
                // linux 和 mac 物理路径
                return str.Replace("~", string.Empty).Replace("\\", "/").Replace("//", "/");
            }
        }

        #endregion

        #endregion

        #region 加密

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MD5Sign(string str, bool type = true)
        {
            using var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                if (type)
                {
                    sb.Append(b.ToString("x2"));
                }
                else
                {
                    sb.Append(b.ToString("X2"));
                }
            }

            var resutl = sb.ToString();
            return resutl;
        }

        /// <summary>
        /// MD5 16位
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static string Md5Sign16(string str, bool isLower = false)
        {
            using var md5 = new MD5CryptoServiceProvider();
            string res = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(str)), 4, 8);
            res = res.Replace("-", "");
            // 是否小写
            if (isLower)
            {
                res = res.ToLower();
            }

            return res;
        }

        /// <summary>
        /// Sha1加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Sha1Encode(string value)
        {
            using SHA1 algorithm = SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }

            return sh1;
        }

        /// <summary>
        /// 对外接口通过私钥
        /// </summary>
        public static Dictionary<string, string> DicSecret = new Dictionary<string, string> { };

        /// <summary>
        /// 时间截
        /// </summary>
        public static Int64 InterfaceTimeStamp
        {
            get
            {
                return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
            }
        }

        /// <summary>
        /// 对外接口通用加密方法
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="timeStamp"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string InterfaceEncrypt(string appKey, string timeStamp, string body)
        {
            if (DicSecret.ContainsKey(appKey))
            {
                var secret = DicSecret[appKey];
                var signStr = appKey + secret + timeStamp + body;
                var res = ComHelper.MD5Sign(signStr).ToUpper();
                return res;
            }

            return string.Empty;
        }

        /// <summary>
        /// 对外接口通用加密方法
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="secret"></param>
        /// <param name="timeStamp"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string InterfaceEncrypt(string appKey, string secret, string timeStamp, string body)
        {
            var signStr = appKey + secret + timeStamp + body;
            var res = ComHelper.MD5Sign(signStr).ToUpper();
            return res;
        }

        #endregion

        #region 枚举

        /// <summary>
        /// 通过枚举值字符串获取枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static T GetEnumValueByStr<T>(string enumValue)
        {
            return (T)Enum.Parse(typeof(T), enumValue);
        }

        /// <summary>
        /// 通过值获取枚举字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetEnumName<T>(int val)
        {
            return Enum.GetName(typeof(T), val);
        }

        /// <summary>
        /// 通过枚举类型获取字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumDic<T>()
        {
            var dic = new Dictionary<int, string>();

            var names = Enum.GetNames(typeof(T));
            foreach (var name in names)
            {
                var key = Convert.ToInt32(GetEnumValueByStr<T>(name));
                dic.Add(key, name);
            }

            return dic;
        }

        /// <summary>
        /// 获取字典描述属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetEnumDesc<T>(int val)
        {
            var res = string.Empty;
            var item = Enum.Parse(typeof(T), val.ToString());
            // 获取描述属性
            return GetEnumDesc(item);
        }

        /// <summary>
        /// 获取字典描述属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetEnumDesc<T>(string val)
        {
            var res = string.Empty;
            var item = Enum.Parse(typeof(T), val);
            // 获取描述属性
            return GetEnumDesc(item);
        }

        /// <summary>
        /// 根据枚举对象 获取描述
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetEnumDesc(object item)
        {
            var res = string.Empty;
            // 获取描述属性
            var attrs = item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                DescriptionAttribute descAttr = attrs[0] as DescriptionAttribute;
                res = descAttr.Description;
            }

            return res;
        }

        /// <summary>
        /// 获取枚举key和描述字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumDicDesc<T>()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            if (!typeof(T).IsEnum)
            {
                return dic;
            }

            string desc = string.Empty;
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                var attrs = item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    DescriptionAttribute descAttr = attrs[0] as DescriptionAttribute;
                    desc = descAttr.Description;
                }

                dic.Add(Convert.ToInt32(item), desc);
            }

            return dic;
        }

        /// <summary>
        /// 获取枚举 Text和描述字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumDicTextDesc<T>()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!typeof(T).IsEnum)
            {
                return dic;
            }

            string desc = string.Empty;
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                var attrs = item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    DescriptionAttribute descAttr = attrs[0] as DescriptionAttribute;
                    desc = descAttr.Description;
                }

                dic.Add(item.ToString(), desc);
            }

            return dic;
        }

        #endregion

        #region 日期时间

        /// <summary>
        /// 月份名称
        /// </summary>
        public static Dictionary<int, string> DicMonName = new Dictionary<int, string> { { 1, "一" }, { 2, "二" }, { 3, "三" }, { 4, "四" }, { 5, "五" }, { 6, "六" }, { 7, "七" }, { 8, "八" }, { 9, "九" }, { 10, "十" }, { 11, "十一" }, { 12, "十二" } };

        /// <summary>
        /// 时间转成时间戳(秒)
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ConvertDateTimeInt(DateTime time)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// Unix时间戳格式转换为DateTime时间格式
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertTimeStampToDate(int unixTimeStamp)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            return dt;
        }

        /// <summary>
        /// 时间转成时间戳(毫秒)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertDateToTimeStampMs(DateTime time)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long timeStamp = (long)(time - startTime).TotalMilliseconds;
            return timeStamp;
        }

        /// <summary>
        /// 时间戳(毫秒)转成时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ConvertTimeStampToDateMs(long time)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            DateTime dt = startTime.AddMilliseconds(time);
            return dt;
        }

        /// <summary>
        /// 日期转换
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DateToString(DateTime? date, string format = "yyyy-MM-dd")
        {
            if (date.HasValue)
            {
                return date.Value.ToString(format);
            }

            return string.Empty;
        }

        /// <summary>
        /// 2个日期相隔多少月
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static int MonSub(DateTime dt1, DateTime dt2)
        {
            var res = (dt1.Year - dt2.Year) * 12 + (dt1.Month - dt2.Month);
            return res;
        }

        /// <summary> 
        /// 计算某天对应的周一
        /// </summary> 
        /// <param name="date">该周中任意一天</param> 
        /// <returns></returns> 
        public static DateTime GetWeekMonday(DateTime date)
        {
            return date.AddDays(1 - Convert.ToInt32(date.DayOfWeek.ToString("d")));
        }

        /// <summary> 
        /// 计算某天对应的周日
        /// </summary> 
        /// <param name="date">该周中任意一天</param> 
        /// <returns></returns> 
        public static DateTime GetWeekSunday(DateTime date)
        {
            var startWeek = GetWeekMonday(date);
            return startWeek.AddDays(6);
        }

        /// <summary>
        /// 获取当前日期对应月出
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetMonStart(DateTime date)
        {
            return date.Date.AddDays(1 - date.Day);
        }

        /// <summary>
        /// 获取当前日期对应的月末
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetMonEnd(DateTime date)
        {
            var monStart = GetMonStart(date);
            return monStart.AddMonths(1).AddDays(-1);
        }

        #endregion

        #region IP

        ///// <summary>
        /////  获取当前机器的ip
        ///// </summary>
        ///// <returns></returns>
        //public static string GetIPAddress()
        //{
        //    string hostName = Dns.GetHostName();
        //    IPAddress[] myIP = Dns.GetHostAddresses(hostName);
        //    IPAddress resultIp = null;
        //    foreach (IPAddress address in myIP)
        //    {
        //        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        //        {
        //            resultIp = address;
        //            break;
        //        }
        //    }

        //    if (resultIp == null)
        //    {
        //        resultIp = myIP[0];
        //    }

        //    return resultIp.ToString();
        //}

        ///// <summary>
        ///// 获取客户端IP地址
        ///// </summary>
        ///// <returns>若失败则返回回送地址</returns>
        //public static string GetClientIP()
        //{
        //    //如果客户端使用了代理服务器，则利用HTTP_X_FORWARDED_FOR找到客户端IP地址
        //    string userHostAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (!string.IsNullOrEmpty(userHostAddress))
        //    {
        //        userHostAddress = userHostAddress.ToString().Split(',')[0].Trim();
        //    }
        //    //否则直接读取REMOTE_ADDR获取客户端IP地址
        //    if (string.IsNullOrEmpty(userHostAddress))
        //    {
        //        userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //    //前两者均失败，则利用Request.UserHostAddress属性获取IP地址，但此时无法确定该IP是客户端IP还是代理IP
        //    if (string.IsNullOrEmpty(userHostAddress))
        //    {
        //        userHostAddress = HttpContext.Current.Request.UserHostAddress;
        //    }
        //    //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
        //    if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
        //    {
        //        return userHostAddress;
        //    }

        //    return "127.0.0.1";
        //}

        ///// <summary>
        ///// 获取客户端真实IP
        ///// </summary>
        ///// <returns></returns>
        //public static string GetRealIP()
        //{
        //    string result = string.Empty;
        //    result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    //可能有代理   
        //    if (!string.IsNullOrWhiteSpace(result))
        //    {
        //        //没有"." 肯定是非IP格式  
        //        if (result.IndexOf(".") == -1)
        //        {
        //            result = null;
        //        }
        //        else
        //        {
        //            //有","，估计多个代理。取第一个不是内网的IP。  
        //            if (result.IndexOf(",") != -1)
        //            {
        //                result = result.Replace(" ", string.Empty).Replace("\"", string.Empty);

        //                string[] temparyip = result.Split(",;".ToCharArray());

        //                if (temparyip != null && temparyip.Length > 0)
        //                {
        //                    for (int i = 0; i < temparyip.Length; i++)
        //                    {
        //                        //找到不是内网的地址  
        //                        if (IsIP(temparyip[i])
        //                            && temparyip[i].Substring(0, 3) != "10."
        //                            && temparyip[i].Substring(0, 7) != "192.168"
        //                            && temparyip[i].Substring(0, 7) != "172.16.")
        //                        {
        //                            return temparyip[i];
        //                        }
        //                    }
        //                }
        //            }
        //            //代理即是IP格式  
        //            else if (IsIP(result))
        //            {
        //                return result;
        //            }
        //            //代理中的内容非IP  
        //            else
        //            {
        //                result = null;
        //            }
        //        }
        //    }

        //    if (string.IsNullOrWhiteSpace(result))
        //    {
        //        result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    }

        //    if (string.IsNullOrWhiteSpace(result))
        //    {
        //        result = HttpContext.Current.Request.UserHostAddress;
        //    }

        //    return result;
        //}

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        #endregion

        #region 金额

        /// <summary>
        /// 大小写转换
        /// </summary>
        /// <param name="nmoney"></param>
        /// <returns></returns>
        public static string MoneyChanageToUpper(double? nmoney)
        {
            if (nmoney.HasValue)
            {
                string money = Math.Abs(nmoney.Value).ToString("f2");
                String[] MyScale = { "分", "角", "元", "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };
                String[] MyBase = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
                String M = nmoney < 0 ? "负" : string.Empty;

                var tmp = string.Empty;
                for (int i = 0; i < money.Length; i++)
                {
                    if (money[i] == '.')
                    {
                        M += "元";
                        tmp = string.Empty;
                    }
                    else
                    {
                        int MyData = Convert.ToInt16(money[i].ToString());
                        if (MyData > 0)
                        {
                            M += tmp + MyBase[MyData];
                            M += MyScale[(money.Length - 1) - i];
                            tmp = string.Empty;
                        }
                        else
                        {
                            tmp = MyBase[0];
                        }
                    }
                }

                return M;
            }

            return string.Empty;
        }

        /// <summary>
        /// 金额转换,默认为ToString("f2")
        /// </summary>
        /// <param name="val"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DoubleToString(double? val, string format = "f2")
        {
            if (val.HasValue)
            {
                return val.Value.ToString(format);
            }

            return string.Empty;
        }

        /// <summary>
        /// 数字转中文大写
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ConvertToChinese(double x)
        {
            string s = x.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", m => "负 空零一二三四五六七八九空空空空空空空分角十百千万亿兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }

        /// <summary>
        /// 数字转中文大写
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ConvertToTradChinese(double x)
        {
            string s = x.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", m => "负 空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }

        /// <summary>
        /// 数字转通用繁体中文
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ConvertToCommSimCh(double x)
        {
            string res = ConvertToChinese(x).Trim();
            if (res == "二")
            {
                return "两";
            }
            if (res.Contains("一十"))
            {
                res = res.Replace("一十", "十");
            }
            return res;
        }

        /// <summary>
        /// 数字转通用繁体中文
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ConvertToCommTradCh(double x)
        {
            string res = ConvertToTradChinese(x).Trim();
            if (res == "贰")
            {
                return "两";
            }
            if (res.Contains("壹拾"))
            {
                res = res.Replace("壹拾", "拾");
            }
            return res;
        }

        #endregion

        #region 缩略图

        /// <summary> 
        /// 为图片生成缩略图
        /// </summary> 
        /// <param name="phyPath">原图片的路径</param> 
        /// <param name="width">缩略图宽</param> 
        /// <param name="height">缩略图高</param> 
        /// <returns></returns> 
        public static Image GetHvtThumbnail(string phyPath, int width, int height)
        {
            if (string.IsNullOrEmpty(phyPath))
            {
                phyPath = "~/Images/upImgDef.jpg";
            }

            using (var image = Image.FromFile(phyPath))
            {
                //代码是从开源项目HoverTreeCMS中获取的
                //更多信息请参考：http://hovertree.com/menu/hovertreecms/
                Bitmap m_hovertreeBmp = new Bitmap(width, height);
                //从Bitmap创建一个System.Drawing.Graphics 
                Graphics m_HvtGr = Graphics.FromImage(m_hovertreeBmp);
                //设置  
                m_HvtGr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //下面这个也设成高质量 
                m_HvtGr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //下面这个设成High 
                m_HvtGr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //把原始图像绘制成上面所设置宽高的缩小图 
                Rectangle rectDestination = new Rectangle(0, 0, width, height);

                int m_width, m_height;
                int srcX = 0;
                int srcY = 0;
                if (image.Width * height > image.Height * width)
                {
                    m_height = image.Height;
                    m_width = (image.Height * width) / height;
                    srcX = (image.Width - m_width) / 2;
                }
                else
                {
                    m_width = image.Width;
                    m_height = (image.Width * height) / width;
                    srcY = (image.Height - m_height) / 2;
                }

                m_HvtGr.DrawImage(image, rectDestination, srcX, srcY, m_width, m_height, GraphicsUnit.Pixel);
                return m_hovertreeBmp;
            }
        }

        #endregion

        #region 锁

        /// <summary>
        /// 锁1
        /// </summary>
        public static object ObjLock1 = new object();

        /// <summary>
        /// 锁2
        /// </summary>
        public static object ObjLock2 = new object();

        /// <summary>
        /// 锁3
        /// </summary>
        public static object ObjLock3 = new object();

        #endregion

        #region 颜色转换

        /// <summary>
        /// 16进制转成RGB
        /// </summary>
        /// <param name="strHxColor">设置16进制颜色(#FFFFFF) </param>
        /// <returns></returns>
        public static Color HexStrToColor(string strHxColor)
        {
            return Color.FromArgb(int.Parse(strHxColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier), int.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier), int.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
        }

        /// <summary>
        /// 系统颜色转RB
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string SysColorToStr(Color color)
        {
            return ColorTranslator.ToHtml(color);
        }

        #endregion

        #region 数组抽取随机数

        /// <summary>
        /// 数据随机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="mulpit"></param>
        /// <returns></returns>
        public static T GetRandomVal<T>(List<T> list, int mulpit = 10)
        {
            // 数组的长度
            var urlCount = list.Count;
            // 随机Seed
            var rand = new Random(Guid.NewGuid().GetHashCode());
            // 随机结果（扩大mulpit倍）
            var randInd = rand.Next(urlCount * mulpit);
            // 余数排序
            var urlInd = (int)Math.Round((double)randInd % (double)urlCount);
            // 数据乱序
            list = list.OrderBy(x => Guid.NewGuid()).ToList();
            // 结果
            var res = list[urlInd];
            return res;
        }

        #endregion

        #region 两int型数相除转换成百分比

        /// <summary>
        /// 两int型数相除转换成百分比
        /// </summary>
        /// <param name="divisorOne"></param>
        /// <param name="divisorTwo"></param>
        /// <returns></returns>
        public static string GetPercentByDivision(int divisorOne, int divisorTwo)
        {
            if (divisorTwo == 0)
            {
                return "0%";
            }
            else
            {
                return Math.Round(Convert.ToDecimal((float)divisorOne * 100 / divisorTwo), 2).ToString() + "%";
            }
        }

        #endregion

        #region 数据转换

        /// <summary>
        /// 字符串分割
        /// </summary>
        /// <param name="str"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] StrSplitList(this string str, string split = ",")
        {
            return !string.IsNullOrEmpty(str) ? str.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
        }

        #endregion

        #region 正则验证

        /// <summary>
        /// 验证手机是否正确
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool CheckPhoneVaild(string phone)
        {
            return Regex.IsMatch(phone, @"^(0|86)?(13|14|15|16|17|18|19)[0-9]{9}$");
        }

        #endregion
    }
}
