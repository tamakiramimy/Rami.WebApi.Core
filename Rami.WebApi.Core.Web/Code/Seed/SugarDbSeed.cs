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

            var sql = @"

#菜单初始化
REPLACE INTO permmenu(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`, `IconClass`, `NeedAuth`) 
VALUES (1, 0, '权限管理', '-', 2, '002', 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin', 'fa-address-card', 1);
REPLACE INTO permmenu(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`, `IconClass`, `NeedAuth`) 
VALUES (2, 1, '菜单管理', '/permission/menu', 1, '002001', 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin', 'fa-address-card', 1);
REPLACE INTO permmenu(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`, `IconClass`, `NeedAuth`) 
VALUES (3, 1, '角色管理', '/permission/role', 2, '002002', 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin', 'fa-address-card', 1);
REPLACE INTO permmenu(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`, `IconClass`, `NeedAuth`) 
VALUES (4, 1, '角色菜单管理', '/permission/rolemenu', 4, '002004', 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin', 'fa-address-card', 1);
REPLACE INTO permmenu(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`, `IconClass`, `NeedAuth`) 
VALUES (5, 1, '用户角色管理', '/permission/userrole', 3, '002003', 0, '2019-11-19 00:00:00', '2019-11-22 13:49:22', 'admin', 'admin', 'fa-qq', 1);

#管理员角色
REPLACE INTO permrole(`Id`, `RoleName`, `Description`, `OrdInd`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) 
VALUES (1, 'admin', '管理员', 1, 0, '2019-11-19 00:00:00', '2019-11-22 11:39:02', 'admin', 'admin');

#管理员角色-菜单
REPLACE INTO permrolemenu(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) 
VALUES (1, 1, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO permrolemenu(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) 
VALUES (1, 2, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO permrolemenu(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) 
VALUES (1, 3, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO permrolemenu(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) 
VALUES (1, 4, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO permrolemenu(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) 
VALUES (1, 5, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');

#增加管理员用户
REPLACE INTO permuserrole(`UserName`, `RId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) 
VALUES ('admin', 1, 0, '2019-11-19 00:00:00', '2019-11-22 18:14:05', 'admin', 'admin');

#查询初始化后数据
SELECT * from permmenu;
SELECT * from permrole;
SELECT * from permrolemenu;
SELECT * from permuserrole;";

            var transRes = works.RunTransaction(() =>
            {
                var initRes = works.ExecuteCommand(sql);
            });
        }
    }
}
