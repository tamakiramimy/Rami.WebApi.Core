using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 缓存
    /// </summary>
    public class CacheHelper : ICacheHelper
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="repo"></param>
        public CacheHelper(IRepository repo)
        {
            this.repo = repo;
        }

        #region 组织架构

        /// <summary>
        /// 用户缓存
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public List<UserInfoShow> GetUserInfoCache()
        {
            return repo.Query<UserInfoShow>(x => x.IsDel == 0);
        }

        #endregion

        #region 菜单角色权限

        /// <summary>
        /// 菜单权限缓存
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public List<PermMenuDto> GetPermMenuCache()
        {
            return repo.Query<PermMenuDto>(x => x.IsDel == 0);
        }

        /// <summary>
        /// 角色缓存
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public List<PermRoleDto> GetPermRoleCache()
        {
            return repo.Query<PermRoleDto>(x => x.IsDel == 0);
        }

        /// <summary>
        /// 角色菜单缓存
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public List<PermRoleMenuDto> GetPermRoleMenuCache()
        {
            return repo.Query<PermRoleMenuDto>(x => x.IsDel == 0);
        }

        /// <summary>
        /// 用户角色缓存
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public List<PermUserRoleDto> GetPermUserRoleCache()
        {
            return repo.Query<PermUserRoleDto>(x => x.IsDel == 0);
        }

        #endregion

        #region 公众号配置

        /// <summary>
        /// 数据库通用配置
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public List<DbConfig> GetDbCommConfig()
        {
            return repo.Query<DbConfig>();
        }

        /// <summary>
        /// 数据库通用配置字典
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public Dictionary<string, DbConfig> DicDbConmmConfig()
        {
            var lstConfigs = GetDbCommConfig();
            return lstConfigs.ToDictionary(x => x.ConfigName);
        }

        /// <summary>
        /// 根据key获取配置值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public string GetDbConfValByKey(string key)
        {
            var dicTemp = DicDbConmmConfig();
            if (dicTemp.ContainsKey(key))
            {
                return dicTemp[key].ConfigValue;
            }

            return string.Empty;
        }

        #endregion

        #region 关注用户的国家、省、市

        /// <summary>
        /// 获取国家、省、市缓存
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 1)]
        public List<CountryInfo> GetCountryCache()
        {
            var strSql = @" SELECT DISTINCT Country,Province ,City FROM WxUserInfo 
WHERE IsSubscribe = 1 AND Country IS NOT NULL AND Country <> '' ";
            return repo.SqlQuery<CountryInfo>(strSql);
        }

        /// <summary>
        /// 根据类型获取 国家、省、市
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        public List<string> GetCountryByName(int type, string pName)
        {
            var lstCountInfo = GetCountryCache();
            if (type == 1)
            {
                return lstCountInfo.Select(x => x.Country).Distinct().OrderByDescending(x => x).ToList();
            }
            else if (type == 2)
            {
                return lstCountInfo.Where(x => x.Country == pName && x.Province != null && x.Province != "")
                    .Select(x => x.Province).Distinct().OrderBy(x => x).ToList();
            }
            else
            {
                return lstCountInfo.Where(x => x.Province == pName && x.City != null && x.City != "")
                    .Select(x => x.City).Distinct().OrderBy(x => x).ToList();
            }
        }

        #endregion
    }
}
