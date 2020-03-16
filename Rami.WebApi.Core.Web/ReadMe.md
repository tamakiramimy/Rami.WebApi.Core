#重建表
DROP TABLE IF EXISTS `permmenu`;
CREATE TABLE `permmenu`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PId` int(11) NULL DEFAULT NULL,
  `MenuName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Router` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OrdInd` int(11) NOT NULL,
  `TreeCode` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `IsDel` int(11) NOT NULL,
  `CreateTime` datetime(0) NOT NULL,
  `UpdateTime` datetime(0) NOT NULL,
  `Creater` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Updater` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `IconClass` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `NeedAuth` int(11) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 12 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

SET FOREIGN_KEY_CHECKS = 1;


DROP TABLE IF EXISTS `permrole`;
CREATE TABLE `permrole`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `OrdInd` int(11) NOT NULL,
  `IsDel` int(11) NOT NULL,
  `CreateTime` datetime(0) NOT NULL,
  `UpdateTime` datetime(0) NOT NULL,
  `Creater` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Updater` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

SET FOREIGN_KEY_CHECKS = 1;


DROP TABLE IF EXISTS `permuserrole`;
CREATE TABLE `permuserrole`  (
  `UserName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RId` int(11) NOT NULL,
  `IsDel` int(11) NOT NULL,
  `CreateTime` datetime(0) NOT NULL,
  `UpdateTime` datetime(0) NOT NULL,
  `Creater` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Updater` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`UserName`, `RId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

SET FOREIGN_KEY_CHECKS = 1;


DROP TABLE IF EXISTS `permrolemenu`;
CREATE TABLE `permrolemenu`  (
  `RId` int(11) NOT NULL,
  `MId` int(11) NOT NULL,
  `IsDel` int(11) NOT NULL,
  `CreateTime` datetime(0) NOT NULL,
  `UpdateTime` datetime(0) NOT NULL,
  `Creater` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Updater` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`RId`, `MId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

SET FOREIGN_KEY_CHECKS = 1;


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
SELECT * from permuserrole;
