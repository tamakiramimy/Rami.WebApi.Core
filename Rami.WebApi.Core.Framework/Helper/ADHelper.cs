using System;
using System.DirectoryServices;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// AD域认证(暂时不支持Linux，改用LdapHelper)
    /// </summary>
    public class ADHelper
    {
        /// <summary>
        /// 日志
        /// </summary>
        private ILogHelper<ADHelper> logHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_logHelper"></param>
        public ADHelper(ILogHelper<ADHelper> _logHelper)
        {
            logHelper = _logHelper;
        }

        /// <summary>
        /// LDAP绑定路径
        /// </summary>
        private string ADPath = ComHelper.GetConf("AppSettings:AdLogin:ADPath");

        /// <summary>
        /// 普通用户调用
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private DirectoryEntry GetDirectoryObject(string userName, string password)
        {
            try
            {
                var adPathList = ADPath.Split('|');
                foreach (var path in adPathList)
                {
                    try
                    {
                        var entry = new DirectoryEntry(path, userName, password, AuthenticationTypes.Secure);
                        Object native = entry.NativeObject;
                        return entry;
                    }
                    catch (Exception ex)
                    {
                        logHelper.Debug($"Ad:GetDirectoryObject出错:{ex.Message}", ex);
                        continue;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logHelper.Debug($"Ad:GetDirectoryObject出错:{ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// AD登陆验证
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        public bool Login(string userName, string userPwd)
        {
            try
            {
                var entry = GetDirectoryObject(userName, userPwd);
                Object native = entry.NativeObject;
                return true;
            }
            catch (Exception ex)
            {
                logHelper.Error($"Ad:Login出错:{ex.Message}", ex);
                return false;
            }
        }
    }
}
