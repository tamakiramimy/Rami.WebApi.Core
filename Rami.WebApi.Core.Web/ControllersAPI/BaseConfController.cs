using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Web.Code;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 通用配置API
    /// </summary>
    [ApiAuthorize(AuthenticationSchemes = AuthHelper.JwtAuthScheme, Policy = AuthConst.ApiAuthSimple)]
    public class BaseConfController : BaseController
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<BaseConfController> logHelper;

        /// <summary>
        /// 当前用户
        /// </summary>
        private readonly CurrentUser currentUser;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="logHelper"></param>
        /// <param name="currentUser"></param>
        public BaseConfController(IRepository repo, ILogHelper<BaseConfController> logHelper, CurrentUser currentUser)
        {
            this.repo = repo;
            this.logHelper = logHelper;
            this.currentUser = currentUser;
        }

        #region 通用配置

        /// <summary>
        /// 获取通用配置
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetDbConfigs")]
        public async Task<Result<Pager<DbConfig>>> GetDbConfigs([FromBody]Para<DbConfig> para)
        {
            try
            {
                var query = para.Entity;
                if (query != null)
                {
                    if (!string.IsNullOrEmpty(query.ConfigRemark))
                    {
                        para.Filter = para.Filter.And(x => x.ConfigRemark.Contains(query.ConfigRemark));
                    }
                }

                var pageRes = await repo.QueryPageAsync(para);
                return new Result<Pager<DbConfig>> { IsSucc = true, Data = pageRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetDbConfigs:获取数据库配置失败：" + ex.Message + "      " + ex.StackTrace);
            }

            return new Result<Pager<DbConfig>> { Message = "获取数据库配置失败！" };
        }

        /// <summary>
        /// 保存通用配置
        /// </summary>
        [HttpPost("SaveDbConfig")]
        public Result<string> SaveDbConfig([FromBody]DbConfig item)
        {
            try
            {
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                repo.Update(item, new List<string> { "Updater", "UpdateTime", "ConfigValue" });
                return new Result<string> { IsSucc = true, Message = "保存通用配置成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetDbConfigs:保存通用配置失败：" + ex.Message + "      " + ex.StackTrace);
            }

            return new Result<string> { Message = "保存通用配置失败！" };
        }

        #endregion
    }
}