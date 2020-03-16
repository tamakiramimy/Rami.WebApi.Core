using Rami.WebApi.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICacheHelper
    {
        #region 组织架构

        /// <summary>
        /// 用户缓存
        /// </summary>
        /// <returns></returns>
        List<UserInfoShow> GetUserInfoCache();

        #endregion

        #region 菜单角色权限

        /// <summary>
        /// 菜单权限缓存
        /// </summary>
        /// <returns></returns>
        List<PermMenuDto> GetPermMenuCache();

        /// <summary>
        /// 角色缓存
        /// </summary>
        /// <returns></returns>
        List<PermRoleDto> GetPermRoleCache();

        /// <summary>
        /// 角色菜单缓存
        /// </summary>
        /// <returns></returns>
        List<PermRoleMenuDto> GetPermRoleMenuCache();

        /// <summary>
        /// 用户角色缓存
        /// </summary>
        /// <returns></returns>
        List<PermUserRoleDto> GetPermUserRoleCache();

        #endregion

        #region 公众号配置

        /// <summary>
        /// 数据库通用配置
        /// </summary>
        /// <returns></returns>
        List<DbConfig> GetDbCommConfig();

        /// <summary>
        /// 数据库通用配置字典
        /// </summary>
        /// <returns></returns>
        Dictionary<string, DbConfig> DicDbConmmConfig();

        /// <summary>
        /// 根据key获取配置值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetDbConfValByKey(string key);

        #endregion

        #region 关注用户的国家、省、市

        /// <summary>
        /// 获取国家、省、市缓存
        /// </summary>
        /// <returns></returns>
        List<CountryInfo> GetCountryCache();

        /// <summary>
        /// 根据类型获取 国家、省、市
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        List<string> GetCountryByName(int type, string pName);

        #endregion
    }
}
