using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// AD认证
    /// </summary>
    public class LdapHelper
    {
        /// <summary>
        /// 域认证地址（ldap.baidu.com）
        /// </summary>
        private string LdapHost => ComHelper.GetConf("LDAP:Host");

        /// <summary>
        /// 域认证端口（默认：389）
        /// </summary>
        private int LdapPort => ComHelper.GetConf("LDAP:Port").ObjToInt();

        /// <summary>
        /// 域邮箱后缀（@baidu.com）
        /// </summary>
        public string MailSuffix => ComHelper.GetConf("LDAP:MailSuffix");

        /// <summary>
        /// 域网段（DC=baidu,DC=group）
        /// </summary>
        public string SearchBase => ComHelper.GetConf("LDAP:SearchBase");

        /// <summary>
        /// 域认证查询字段（displayname|mail|sn）
        /// </summary>
        public static string[] Attrs => ComHelper.GetConf("LDAP:Attrs").StrSplitList();

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<LdapHelper> logHelper;

        /// <summary>
        /// AD认证
        /// </summary>
        /// <param name="logHelper"></param>
        public LdapHelper(ILogHelper<LdapHelper> logHelper)
        {
            this.logHelper = logHelper;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool LdapAuthenticate(string username, string password)
        {
            var loginDN = username;
            var sAMAccountName = username;
            if (username.Contains(MailSuffix))
                sAMAccountName = username.Substring(0, username.IndexOf(MailSuffix));
            else
                loginDN = $"{username}{MailSuffix}";

            var searchFilter = $"(sAMAccountName={sAMAccountName})";
            try
            {
                var conn = new LdapConnection();
                conn.Connect(LdapHost, LdapPort);
                conn.Bind(loginDN, password);
                var lsc = conn.Search(SearchBase, LdapConnection.SCOPE_SUB, searchFilter, Attrs, false);

                while (lsc.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = lsc.Next();
                        if (nextEntry != null)
                        {
                            break;
                        }
                    }
                    catch (LdapException ex)
                    {
                        logHelper.Error("LdapAuthenticate:ad认证出错:" + ex.Message + "     " + ex.StackTrace);
                        continue;
                    }
                }

                conn.Disconnect();
                return true;
            }
            catch (Exception ex1)
            {
                logHelper.Error("LdapAuthenticate:ad认证出错:" + ex1.Message + "     " + ex1.StackTrace);
            }

            return false;
        }
    }
}
