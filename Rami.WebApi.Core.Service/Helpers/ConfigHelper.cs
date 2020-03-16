using Rami.WebApi.Core.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 通用配置帮助类
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 是否测试
        /// </summary>
        public static bool IsDev
        {
            get
            {
                var conf = ComHelper.GetConf("AppSettings:IsDev");
                if (!string.IsNullOrWhiteSpace(conf) && conf == "1")
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Cookie认证Cookie名称
        /// </summary>
        public static string CookieAuthName = ComHelper.GetConf("AppSettings:CookieAuthName");

        /// <summary>
        /// Jwt授权Cookie名称
        /// </summary>
        public static string JwtAuthName = ComHelper.GetConf("AppSettings:JwtAuthName");

        /// <summary>
        /// 是否Oidc认证；是：Oidc认证，否：Jwt(api)+Cookies（mvc）
        /// </summary>
        public static bool IsOidcAuth = ComHelper.GetConf("AppSettings:IsOidcAuth").ObjToBool();

        /// <summary>
        /// 是否启用API请求响应日志
        /// </summary>
        public static bool IsApiReqResMidLog = ComHelper.GetConf("AppSettings:ApiReqResMidLog:Enabled").ObjToBool();

        /// <summary>
        /// 是否启用AOP Memory缓存
        /// </summary>
        public static bool IsMemoryAopEnabled = ComHelper.GetConf("AppSettings:MemoryCachingAOP:Enabled").ObjToBool();

        /// <summary>
        /// 是否启用AOP Redis缓存
        /// </summary>
        public static bool IsRedisAopEnabled = ComHelper.GetConf("AppSettings:RedisCachingAOP:Enabled").ObjToBool();

        /// <summary>
        /// 是否启用AOP 方法日志
        /// </summary>
        public static bool IsLogAopEnabled = ComHelper.GetConf("AppSettings:LogAOP:Enabled").ObjToBool();

        /// <summary>
        /// 是否还原数据库
        /// </summary>
        public static bool IsDbSeedEnabled = ComHelper.GetConf("AppSettings:IsDbSeedEnabled").ObjToBool();

        /// <summary>
        /// Jwt颁发者
        /// </summary>
        public static string JwtIssuer = ComHelper.GetConf("Audience:Issuer");

        /// <summary>
        /// Jwt授权者
        /// </summary>
        public static string JwtAudience = ComHelper.GetConf("Audience:Audience");

        /// <summary>
        /// Jwt秘钥
        /// </summary>
        public static string JwtSecret = ComHelper.GetConf("Audience:Secret");

        /// <summary>
        /// Jwt期限(秒)
        /// </summary>
        public static int JwtOverSec = ComHelper.GetConf("Audience:OverSec").ObjToInt();

        /// <summary>
        /// 跨域请求
        /// </summary>
        public static string[] AllowOrigins = ComHelper.GetConf("Cors:AllowOrigins").StrSplitList();

        /// <summary>
        /// 启动首页
        /// </summary>
        public static string StartUrl = ComHelper.GetConf("AppSettings:StartUrl");
    }
}
