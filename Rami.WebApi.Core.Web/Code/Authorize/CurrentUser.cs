using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 当前用户
    /// </summary>
    public class CurrentUser
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<CurrentUser> logHelper;

        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 全局用户
        /// </summary>
        private IUser appUser;

        /// <summary>
        /// 缓存
        /// </summary>
        private ICacheHelper cache;

        /// <summary>
        /// Ad域认证
        /// </summary>
        private LdapHelper adHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        public CurrentUser(ILogHelper<CurrentUser> logHelper, IRepository repo, IUser appUser, ICacheHelper cache, LdapHelper adHelper)
        {
            this.logHelper = logHelper;
            this.repo = repo;
            this.appUser = appUser;
            this.cache = cache;
            this.adHelper = adHelper;
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<Result<LoginParam>> Login(LoginModel login)
        {
            try
            {
                var user = await repo.FirstOrDefaultAsync<UserInfoShow>(x => x.UserName == login.UserName);
                if (user != null && ComHelper.MD5Sign(login.Password) == user.UserPwd)
                {
                    var jwtInfo = new TokenModelJwt { };
                    jwtInfo.Uid = user.Id;
                    jwtInfo.UserName = user.UserName;
                    // 查询用户的角色
                    var lstRoles = await repo.QueryAsync<PermUserRoleDto>(x => x.IsDel == 0 && x.UserName == user.UserName);
                    jwtInfo.Role = string.Join(',', lstRoles.Select(x => x.RId).Distinct().ToList());

                    var token = await AuthHelper.GetJwtToken(jwtInfo);
                    return new Result<LoginParam> { IsSucc = true, Data = token };
                }
            }
            catch (Exception ex)
            {
                logHelper.Error($"Login:登陆失败:{ex.Message}", ex);
            }

            return new Result<LoginParam> { Message = "登录失败，请重试！" };
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public async Task Logout()
        {
            await AuthHelper.IdentifyLogout();
        }

        #region 用户信息

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName => appUser.Name;

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<string> LstUserRoles => appUser.LstRoles;

        /// <summary>
        /// 当前用户
        /// </summary>
        public UserInfoShow CurUser
        {
            get
            {
                return cache.GetUserInfoCache().FirstOrDefault(x => x.UserName == UserName);
            }
        }

        /// <summary>
        /// 角色
        /// </summary>
        public string UserRolesStr
        {
            get
            {
                if (ConfigHelper.IsOidcAuth)
                {
                    // 从用户表查询用户角色
                    var lstRoles = cache.GetPermUserRoleCache().Where(x => x.UserName == UserName);
                    return string.Join(",", lstRoles.Select(x => x.RId));
                }
                else
                {
                    // 从认证Clamis查询用户角色
                    return appUser.Roles;
                }
            }
        }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserIcon
        {
            get
            {
                var uTemp = CurUser;
                if (!string.IsNullOrEmpty(uTemp.UserIcon))
                {
                    return uTemp.UserIcon;
                }
                else
                {
                    return ComHelper.GetAbsPath("Images/DefIcon.jpg");
                }
            }
        }

        #endregion

        #region 用户角色

        /// <summary>
        /// 判断用户是否包含角色
        /// </summary>
        /// <param name="strRoles"></param>
        /// <returns></returns>
        public bool IsHasRole(string strRoles)
        {
            var lstRoles = strRoles.StrSplitList();
            if (lstRoles == null)
            {
                return false;
            }

            return IsHasRole(lstRoles);
        }

        /// <summary>
        /// 是否有权限
        /// </summary>
        /// <param name="lstRoles"></param>
        /// <returns></returns>
        public bool IsHasRole(params EnumSysRoles[] lstRoles)
        {
            if (lstRoles.Length == 0)
            {
                return false;
            }

            var temp = lstRoles.Select(x => ((int)x).ToString()).ToArray();
            return IsHasRole(temp);
        }

        /// <summary>
        /// 判断用户是否包含角色
        /// </summary>
        /// <param name="lstRoles"></param>
        /// <returns></returns>
        public bool IsHasRole(params string[] lstRoles)
        {
            var sysRoles = "," + UserRolesStr + ",".ToLower();
            foreach (var role in lstRoles)
            {
                var rolesSearch = "," + role + ",".ToLower();
                if (sysRoles.IndexOf(rolesSearch) > -1)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
