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
    ///  还原数据库
    /// </summary>
    public class SugarDbSeed
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        private readonly IRepository works;

        /// <summary>
        /// 构造方法
        /// </summary>
        public SugarDbSeed(IRepository works)
        {
            this.works = works;
        }

        /// <summary>
        /// 异步添加种子数据
        /// </summary>
        /// <returns></returns>
        public async Task SeedAsync()
        {
            try
            {
                // 如果生成过了，第二次，就不用再执行一遍了,修改配置文件IsDbSeedEnabled=false;即可
                if (ConfigHelper.IsDbSeedEnabled)
                {
                    // 创建表
                    works.Context.CreateTableByEntity(false,
                        typeof(UserInfo),

                        typeof(PermMenu),
                        typeof(PermRole),
                        typeof(PermRoleMenu),
                        typeof(PermUserRole),

                        typeof(WxAutoKeyword),
                        typeof(WxAutoKeywordDetail),
                        typeof(WxAutoPush),
                        typeof(WxAutoPushDetail),
                        typeof(WxKfInfo),
                        typeof(WxMedia),
                        typeof(WxMenus),
                        typeof(WxNews),
                        typeof(WxNewsDetail),
                        typeof(WxQrCodeLog),
                        typeof(WxSubScribeLog),
                        typeof(WxUserInfo),
                        typeof(WxUserInfoExt),
                        typeof(WxUserOptLog),
                        typeof(WxBackNews),

                        typeof(DbConfig)
                        );

                    //// 初始化数据
                    //InitDatas();
                }

                await Task.Delay(1);
            }
            catch (Exception ex)
            {
                throw new Exception("1、注意要先创建空的数据库\n2、错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitDatas()
        {
            // 这样写代码量太多，改成Sql方式
            //var dtNow = DateTime.Now;
            //var m1 = new PermMenu
            //{
            //    Id = 1,
            //    PId = 0,
            //    MenuName = "权限管理",
            //    Router = "-",
            //    OrdInd = 2,
            //    TreeCode = "002",
            //    IconClass = " fa-address-card ",
            //    NeedAuth = 1,
            //    Creater = "system",
            //    Updater = "system",
            //    CreateTime = dtNow,
            //    UpdateTime = dtNow
            //};

            //var addRes = works.Add(m1);

            // 初始化脚本在ReadMe.md
            //var sql = @"";

            //var transRes = works.RunTransaction(() =>
            //{
            //    var initRes = works.ExecuteCommand(sql);
            //});
        }
    }
}
