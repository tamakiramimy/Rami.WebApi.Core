#初始化设置

#还原nuget、编译、运行
dotnet restore
dotnet build
dotnet run

#配置好 公众号配置文件 Wechat.json 相关参数 

#如果vue项目和webapi单独部署，设置appsettings.json StartUrl 为 swagger 可以查看api列表
#如果vue项目和webapi一起部署，设置appsettings.json StartUrl 为 backstage.html 可以修改首页为公众号后台管理登录界面

#创建本地mysql数据库
#修改appsettings.json 数据库连接，修改IsDbSeedEnabled 为 true；
#调用api/Permission/SeedData 生成库表
#修改IsDbSeedEnabled 为 false；
#执行以下脚本生成基础数据

#菜单初始化
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 0, '权限管理', '-', 1, '001', 'fa-qq', 1, 0, 0, 0, '2019-11-19 00:00:00', '2020-01-19 10:01:22', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 1, '菜单管理', '/permission/menu', 1, '001001', 'fa-address-card', 1, 0, 0, 0, '2019-11-19 00:00:00', '2020-01-19 10:01:46', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 1, '角色管理', '/permission/role', 2, '001002', 'fa-address-card', 1, 0, 0, 0, '2019-11-19 00:00:00', '2020-01-19 10:01:51', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (4, 1, '角色菜单管理', '/permission/rolemenu', 3, '001003', 'fa-address-card', 1, 0, 0, 0, '2019-11-19 00:00:00', '2020-01-19 10:11:56', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (5, 1, '用户角色管理', '/permission/userrole', 4, '001004', 'fa-address-card', 1, 0, 0, 0, '2019-11-19 00:00:00', '2020-01-19 10:11:43', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (6, 0, '公众号配置', '-', 3, '003', 'fa-qq', 1, 0, 0, 0, '2020-01-16 12:02:56', '2020-01-16 12:18:51', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (7, 6, '永久素材管理', '/wxfunc/medialist', 2, '003002', 'fa-address-card', 1, 0, 0, 0, '2020-01-16 12:04:26', '2020-03-14 12:24:54', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (8, 6, '永久图文管理', '/wxfunc/wxnews', 3, '003003', 'fa-address-card', 1, 0, 0, 0, '2020-01-16 12:05:10', '2020-03-14 12:25:03', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (9, 6, '公众号菜单管理', '/wxfunc/wxmenus', 1, '003001', 'fa-address-card', 1, 0, 0, 0, '2020-01-16 12:06:49', '2020-03-14 12:24:47', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (10, 0, '基本设置', '-', 2, '002', 'fa-qq', 1, 0, 0, 0, '2020-01-19 10:00:49', '2020-01-19 10:00:49', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (11, 10, '通用设置', '/baseconf/dbconfigs', 1, '002001', 'fa-address-card', 1, 0, 0, 0, '2020-01-19 10:03:23', '2020-01-19 10:04:10', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (12, 10, '自动推送配置', '/baseconf/autopushconfig', 3, '002003', 'fa-address-card', 1, 0, 0, 0, '2020-01-19 10:06:11', '2020-01-19 10:13:43', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (13, 10, '关键字自动应答', '/baseconf/wxautorespconf', 2, '002002', 'fa-address-card', 1, 0, 0, 0, '2020-01-19 10:07:18', '2020-01-19 10:07:36', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (14, 6, '后台图文管理', '/wxfunc/backnews', 4, '003004', 'fa-address-card', 1, 0, 0, 0, '2020-01-19 10:10:06', '2020-01-19 10:14:28', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (15, 0, '在线客服', '-', 4, '004', 'fa-qq', 1, 0, 0, 0, '2020-01-19 10:17:09', '2020-01-19 10:17:09', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (16, 15, '公众号客服管理', '/wxfunc/wxkflist', 1, '004001', 'fa-address-card', 1, 0, 0, 0, '2020-01-19 10:19:49', '2020-01-21 09:38:05', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (17, 6, '永久图文编辑', '/wxfunc/wxnewsedit', 99, '003099', 'fa-address-card', 1, 1, 0, 0, '2020-01-19 15:21:07', '2020-03-09 12:27:23', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (18, 6, '后台图文编辑', '/wxfunc/backnewsedit', 98, '003098', 'fa-address-card', 1, 1, 0, 0, '2020-01-20 17:07:08', '2020-01-20 17:07:14', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (19, 10, '关键字自动应答-编辑', '/baseconf/wxautorespedit', 4, '002004', 'fa-address-card', 1, 1, 0, 0, '2020-02-26 13:02:13', '2020-02-26 13:02:22', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (20, 10, '自动推送配置-编辑', '/baseconf/autopushedit', 5, '002005', 'fa-address-card', 1, 1, 0, 0, '2020-02-26 18:19:04', '2020-02-26 18:19:10', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (21, 15, '公众号客服登录', 'https://mpkf.weixin.qq.com/', 2, '004002', 'fa-address-card', 0, 0, 1, 0, '2020-02-28 10:18:22', '2020-02-28 10:22:18', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (22, 6, '图文预览', '/wxfunc/newspreview', 97, '003097', 'fa-address-card', 1, 1, 0, 0, '2020-03-14 12:54:55', '2020-03-14 12:55:03', 'admin', 'admin');
REPLACE INTO `PermMenu`(`Id`, `PId`, `MenuName`, `Router`, `OrdInd`, `TreeCode`, `IconClass`, `NeedAuth`, `IsHide`, `IsUrl`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (23, 1, '用户管理', '/permission/users', 0, '001000', 'fa-address-card', 1, 0, 0, 0, '2020-03-16 17:00:17', '2020-03-16 17:01:35', 'admin', 'admin');;

#管理员角色
REPLACE INTO `PermRole`(`Id`, `RoleName`, `Description`, `OrdInd`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 'superadmin', '超级管理员', 1, 0, '2019-11-19 00:00:00', '2020-03-05 11:26:03', 'admin', 'admin');
REPLACE INTO `PermRole`(`Id`, `RoleName`, `Description`, `OrdInd`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 'wxadmin', '管理员', 2, 0, '2020-03-05 11:26:10', '2020-03-05 11:26:10', 'admin', 'admin');
REPLACE INTO `PermRole`(`Id`, `RoleName`, `Description`, `OrdInd`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 'wxke', '微信客服', 3, 0, '2020-03-05 11:26:22', '2020-03-05 11:26:22', 'admin', 'admin');;

#管理员角色-菜单
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 1, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 2, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 3, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 4, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 5, 0, '2019-11-19 00:00:00', '2019-11-19 00:00:00', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 6, 0, '2020-01-16 12:07:42', '2020-01-16 12:07:42', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 7, 0, '2020-01-16 12:07:42', '2020-01-16 12:07:42', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 8, 0, '2020-01-16 12:07:42', '2020-01-16 12:07:42', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 9, 0, '2020-01-16 12:07:42', '2020-01-16 12:07:42', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 10, 0, '2020-01-19 10:12:21', '2020-01-19 10:12:21', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 11, 0, '2020-01-19 10:12:21', '2020-01-19 10:12:21', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 12, 0, '2020-01-19 10:12:21', '2020-01-19 10:12:21', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 13, 0, '2020-01-19 10:12:21', '2020-01-19 10:12:21', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 14, 0, '2020-01-19 10:12:21', '2020-01-19 10:12:21', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 15, 0, '2020-01-19 10:20:18', '2020-01-19 10:20:18', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 16, 0, '2020-01-19 10:20:18', '2020-01-19 10:20:18', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 17, 0, '2020-01-19 15:21:36', '2020-01-19 15:21:36', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 18, 0, '2020-01-20 17:07:24', '2020-01-20 17:07:24', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 19, 0, '2020-02-26 13:02:31', '2020-02-26 13:02:31', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 20, 0, '2020-02-26 18:19:18', '2020-02-26 18:19:18', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 21, 0, '2020-02-28 10:18:30', '2020-02-28 10:18:30', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 22, 0, '2020-03-14 12:55:16', '2020-03-14 12:55:16', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 23, 0, '2020-03-16 17:00:30', '2020-03-16 17:00:30', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 1, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 5, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 6, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 7, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 8, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 9, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 10, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 11, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 12, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 13, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 14, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 15, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 16, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 17, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 18, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 19, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 20, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 21, 0, '2020-03-05 11:26:43', '2020-03-05 11:26:43', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 22, 0, '2020-03-14 12:55:21', '2020-03-14 12:55:21', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (2, 23, 0, '2020-03-16 17:00:40', '2020-03-16 17:00:40', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 6, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 7, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 8, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 9, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 10, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 11, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 12, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 13, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 14, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 15, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 16, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 17, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 18, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 19, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 20, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 21, 0, '2020-03-05 11:26:54', '2020-03-05 11:26:54', 'admin', 'admin');
REPLACE INTO `PermRoleMenu`(`RId`, `MId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (3, 22, 0, '2020-03-14 12:55:25', '2020-03-14 12:55:25', 'admin', 'admin');

#增加管理员用户
REPLACE INTO `UserInfo`(`Id`, `UserName`, `UserPwd`, `Name`, `Email`, `Phone`, `UserIcon`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES (1, 'admin', '21232f297a57a5a743894a0e4a801fc3', '管理员', 'a@b.c', '12345678910', NULL, 0, '2020-03-16 00:00:00', '2020-03-16 17:02:30', 'system', 'admin');

#增加管理员角色
REPLACE INTO `PermUserRole`(`UserName`, `RId`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES ('admin', 1, 0, '2019-11-19 00:00:00', '2019-11-22 18:14:05', 'admin', 'admin');

#公众号基础设置
REPLACE INTO `DbConfig`(`ConfigName`, `ConfigValue`, `ConfigRemark`, `ConfigTag`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES ('kfkeywords', '客服、人工、gotoservices、GoToServices', '联系客服的关键字', '多个关键字请用、隔开', 0, '2018-07-25 11:26:26', '2020-01-19 11:17:24', 'system', 'admin');
REPLACE INTO `DbConfig`(`ConfigName`, `ConfigValue`, `ConfigRemark`, `ConfigTag`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES ('kfsuffix', 'wechatkf', '客服后缀名(公众号绑定微信号)', '10个英文字母或数字', 0, '2018-07-19 11:21:54', '2019-02-19 15:52:52', 'system', 'admin');
REPLACE INTO `DbConfig`(`ConfigName`, `ConfigValue`, `ConfigRemark`, `ConfigTag`, `IsDel`, `CreateTime`, `UpdateTime`, `Creater`, `Updater`) VALUES ('subscribeText', '{nickname}，谢谢你长那么好看还关注我们 ^_^', '首次关注时提示语', '用户昵称请用{nickname}代替', 0, '2018-07-11 16:52:21', '2020-01-19 11:17:40', 'system', 'admin');