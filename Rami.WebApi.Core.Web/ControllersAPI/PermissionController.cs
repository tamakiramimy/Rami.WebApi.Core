using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Rami.WebApi.Core.Web.Code;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 权限管理
    /// </summary>
    [ApiAuthorize(AuthenticationSchemes = AuthHelper.JwtAuthScheme, Policy = AuthConst.ApiAuthSimple)]
    public class PermissionController : BaseController
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository works;

        /// <summary>
        /// 用户
        /// </summary>
        private readonly CurrentUser currentUser;

        /// <summary>
        /// 公用业务
        /// </summary>
        private readonly ComBll comBll;

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly ICacheHelper cacheHelper;

        /// <summary>
        /// 数据库还原
        /// </summary>
        private readonly SugarDbSeed sugarDbSeed;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<PermissionController> logHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="works"></param>
        /// <param name="currentUser"></param>
        /// <param name="comBll"></param>
        /// <param name="cacheHelper"></param>
        /// <param name="sugarDbSeed"></param>
        /// <param name="logHelper"></param>
        public PermissionController(IRepository works, CurrentUser currentUser, ComBll comBll, ICacheHelper cacheHelper,
            SugarDbSeed sugarDbSeed, ILogHelper<PermissionController> logHelper)
        {
            this.works = works;
            this.currentUser = currentUser;
            this.comBll = comBll;
            this.cacheHelper = cacheHelper;
            this.sugarDbSeed = sugarDbSeed;
            this.logHelper = logHelper;
        }


        #region 用户管理

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetUsers")]
        public async Task<Result<Pager<UserInfoShow>>> GetUsers([FromBody]Para<UserInfoShow> para)
        {
            var query = para.Entity;
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.UserName))
                {
                    para.Filter = para.Filter.And(x => x.UserName.Contains(query.UserName));
                }

                if (!string.IsNullOrEmpty(query.Name))
                {
                    para.Filter = para.Filter.And(x => x.Name.Contains(query.Name));
                }
            }

            // 超级管理员
            if (!currentUser.IsHasRole(EnumSysRoles.SuperAdmin))
            {
                para.SQL = $" SELECT * FROM userinfo u WHERE NOT EXISTS (SELECT 1 FROM PermUserRole ur WHERE ur.UserName = u.UserName AND ur.RId = {(int)EnumSysRoles.SuperAdmin})  ";
            }

            para.OrderKey = " IsDel Asc,UpdateTime Desc ";
            var pageRes = await works.QueryPageAsync(para);
            return new Result<Pager<UserInfoShow>> { IsSucc = true, Message = "查询用户列表成功！", Data = pageRes };
        }

        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveUser")]
        public async Task<Result> SaveUser([FromBody]UserInfoShow item)
        {
            // 通用信息
            var dtNow = DateTime.Now;
            var userName = currentUser.UserName;
            item.UpdateTime = dtNow;
            item.Updater = userName;

            if (item.Id == 0 || !item.CreateTime.HasValue)
            {
                // 新增判断用户名是否重复
                var exist = await works.FirstOrDefaultAsync<UserInfoShow>(x => x.UserName == item.UserName);
                if (exist != null)
                {
                    return new Result { Message = "用户登录名称已经存在，请使用其他名称！" };
                }

                // 密码MD5
                item.UserPwd = ComHelper.MD5Sign(item.UserPwd);
                item.CreateTime = dtNow;
                item.Creater = userName;
                await works.AddAsync(item);
            }
            else
            {
                // 保存
                await works.UpdateAsync(item, lstIgnoreColumns: new List<string> { "UserName", "UserPwd" });
            }

            return new Result { IsSucc = true, Message = $"保存用户成功！" };
        }

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("UpdUserPwd")]
        public async Task<Result> UpdUserPwd([FromBody]UserInfoShow item)
        {
            var dtNow = DateTime.Now;
            item.IsDel = 1 - item.IsDel;
            item.UpdateTime = dtNow;
            item.Updater = currentUser.UserName;
            // 密码MD5
            item.UserPwd = ComHelper.MD5Sign(item.UserPwd);

            var isSucc = await works.UpdateAsync(item, new List<string> { "IsDel", "UpdateTime", "Updater", "UserPwd" });
            return new Result { IsSucc = true, Message = $"更新用户密码{(isSucc ? "成功！" : "失败！")}" };
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("UpdUser")]
        public async Task<Result> UpdRole([FromBody]UserInfoShow item)
        {
            bool isSucc = await comBll.CommUpd(item);
            return new Result { IsSucc = isSucc, Message = $"更新用户状态{(isSucc ? "成功！" : "失败！")}" };
        }

        #endregion

        #region 菜单管理

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetMenus")]
        public async Task<Result<Pager<PermMenuDto>>> GetMenus([FromBody]Para<PermMenuDto> para)
        {
            var query = para.Entity;
            if (query != null)
            {
                if (query.Id > 0)
                {
                    para.Filter = para.Filter.And(x => x.Id == query.Id);
                }

                if (query.PId > 0)
                {
                    para.Filter = para.Filter.And(x => x.PId == query.PId);
                }

                if (!string.IsNullOrEmpty(query.MenuName))
                {
                    para.Filter = para.Filter.And(x => x.MenuName.Contains(query.MenuName));
                }
            }

            para.OrderKey = " IsDel Asc,TreeCode Asc ";
            var pageRes = await works.QueryPageAsync(para);

            // 拼凑树节点
            var lstAllMenu = await works.QueryAsync<PermMenuDto>(x => x.IsDel == 0);
            var dicAllMenu = lstAllMenu.ToDictionary(x => x.Id);
            GetMenuFullPath(pageRes, dicAllMenu);

            return new Result<Pager<PermMenuDto>> { IsSucc = true, Message = "获取菜单列表成功！", Data = pageRes };
        }

        /// <summary>
        /// 获取树的全路径
        /// </summary>
        /// <param name="pageRes"></param>
        /// <param name="dicAllMenu"></param>
        private static void GetMenuFullPath(Pager<PermMenuDto> pageRes, Dictionary<int, PermMenuDto> dicAllMenu)
        {
            var rootName = "根目录";
            foreach (var item in pageRes.Datas)
            {
                // 父节点是根目录
                if (item.PId == 0)
                {
                    item.PIdArr = new List<int> { item.PId };
                    item.FullPath = $"{rootName}/{item.MenuName}";
                    continue;
                }

                // 递归找父节点
                if (item.PId > 0)
                {
                    var pIdArr = new List<int> { };
                    var pNameArr = new List<string> { };
                    var curPId = item.PId;
                    pNameArr.Add(item.MenuName);
                    while (curPId > 0)
                    {
                        if (dicAllMenu.ContainsKey(curPId))
                        {
                            var pNode = dicAllMenu[curPId];
                            pIdArr.Add(pNode.Id);
                            pNameArr.Add(pNode.MenuName);
                            curPId = pNode.PId;
                            if (pNode.PId == 0)
                            {
                                pIdArr.Add(pNode.PId);
                                pNameArr.Add(rootName);
                                pIdArr.Reverse();
                                pNameArr.Reverse();
                                item.PIdArr = pIdArr;
                                item.FullPath = string.Join('/', pNameArr);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPermissionTree")]
        public async Task<Result<PermissionTree>> GetPermissionTree()
        {
            var permissions = await works.QueryAsync<PermMenuDto>(x => x.IsDel == 0);
            var permissionTrees = permissions.Where(x => x.IsDel == 0)
                .Select(x => new PermissionTree
                {
                    value = x.Id,
                    label = x.MenuName,
                    Pid = x.PId,
                    //isbtn = x.IsButton,
                    order = x.OrdInd,
                }).ToList();

            var rootNode = new PermissionTree
            {
                value = 0,
                Pid = 0,
                label = "根节点"
            };

            permissionTrees = permissionTrees.OrderBy(d => d.order).ToList();
            TreeHelper.LoopMenuTreeAppendChildren(permissionTrees, rootNode);
            return new Result<PermissionTree> { IsSucc = true, Data = rootNode };
        }

        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveMenu")]
        public async Task<Result> SaveMenu([FromBody]PermMenuDto item)
        {
            await comBll.CommSave(item);
            return new Result { IsSucc = true, Message = $"保存菜单成功！" };
        }

        /// <summary>
        /// 更新菜单状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("UpdMenu")]
        public async Task<Result> UpdMenu([FromBody]PermMenuDto item)
        {
            bool isSucc = await comBll.CommUpd(item);
            return new Result { IsSucc = isSucc, Message = $"更新菜单状态{(isSucc ? "成功！" : "失败！")}" };
        }

        /// <summary>
        /// 根据用户查询菜单
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("GetMenusByUser")]
        [AllowAnonymous]
        public async Task<Result<NavigationBar>> GetMenusByUser([FromQuery]string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new Result<NavigationBar> { Message = "token无效，请重新登录！" };
            }

            var tokenModel = AuthHelper.DecryptJwtToken(token);
            //if (tokenModel != null && tokenModel.Uid > 0)
            //{
            //    // 判断是否当前用户
            //    if (tokenModel.UserName == appUser.Name)
            //    {
            // 判断用户是否存在
            var user = await works.FirstOrDefaultAsync<UserInfoShow>(x => x.UserName == tokenModel.UserName);
            if (user != null)
            {
                // 根据用户获取角色
                var lstUserRoles = await works.QueryAsync<PermUserRole>(x => x.UserName == user.UserName);
                var lstRIds = lstUserRoles.Select(x => x.RId).ToList();
                var lstPerMenus = await works.QueryAsync<PermRoleMenu>(x => lstRIds.Contains(x.RId));
                var lstMIds = lstPerMenus.Select(x => x.MId).ToList();
                var lstMenus = await works.QueryAsync<PermMenuDto>(x => lstMIds.Contains(x.Id));

                var lstNavs = lstMenus.Select(x => new NavigationBar
                {
                    id = x.Id,
                    name = x.MenuName,
                    pid = x.PId,
                    order = x.OrdInd,
                    path = x.Router,
                    iconCls = x.IconClass,
                    //Func = x.Func,
                    IsHide = x.IsHide == 1,
                    IsUrl = x.IsUrl == 1,
                    //IsButton = x.IsButton.ObjToBool(),
                    meta = new NavigationBarMeta
                    {
                        requireAuth = true,
                        title = x.MenuName,
                        //NoTabPage = x.IsHide.ObjToBool()
                    }
                }).ToList();

                // 递归菜单
                var root = new NavigationBar
                {
                    id = 0,
                    pid = 0,
                    order = 0,
                    name = "根节点",
                    path = "",
                    iconCls = "",
                    meta = new NavigationBarMeta(),
                };

                lstNavs = lstNavs.OrderBy(x => x.order).ToList();
                TreeHelper.LoopNaviBarAppendChildren(lstNavs, root);

                return new Result<NavigationBar> { IsSucc = true, Data = root };
                //    }
                //}
            }

            return new Result<NavigationBar> { Data = null };
        }

        #endregion

        #region 角色管理

        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetRoles")]
        public async Task<Result<Pager<PermRoleDto>>> GetRoles([FromBody]Para<PermRoleDto> para)
        {
            var query = para.Entity;
            if (query != null)
            {
                if (query.Id > 0)
                {
                    para.Filter = para.Filter.And(x => x.Id == query.Id);
                }

                if (!string.IsNullOrEmpty(query.RoleName))
                {
                    para.Filter = para.Filter.And(x => x.RoleName.Contains(query.RoleName));
                }

                if (!string.IsNullOrEmpty(query.Description))
                {
                    para.Filter = para.Filter.And(x => x.Description.Contains(query.Description));
                }
            }

            para.OrderKey = " IsDel Asc,OrdInd Asc ";
            var pageRes = await works.QueryPageAsync(para);
            return new Result<Pager<PermRoleDto>> { IsSucc = true, Message = "查询角色列表成功！", Data = pageRes };
        }

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveRole")]
        public async Task<Result> SaveRole([FromBody]PermRoleDto item)
        {
            await comBll.CommSave(item);
            return new Result { IsSucc = true, Message = $"保存角色成功！" };
        }

        /// <summary>
        /// 更新角色状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("UpdRole")]
        public async Task<Result> UpdRole([FromBody]PermRoleDto item)
        {
            bool isSucc = await comBll.CommUpd(item);
            return new Result { IsSucc = isSucc, Message = $"更新角色状态{(isSucc ? "成功！" : "失败！")}" };
        }

        /// <summary>
        /// 获取可用角色列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetRolesUse")]
        public async Task<Result<List<PermRoleDto>>> GetRolesUse()
        {
            var para = new Para<PermRoleDto> { };
            para.Filter = para.Filter.And(x => x.IsDel == 0);
            para.OrderKey = " IsDel Asc,OrdInd Asc ";
            var lstData = await works.QueryAsync(para);
            return new Result<List<PermRoleDto>> { IsSucc = true, Message = "获取可用角色列表成功！", Data = lstData };
        }

        #endregion

        #region 用户-角色管理

        /// <summary>
        /// 查询用户角色列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetUserRoles")]
        public async Task<Result<Pager<PermUserRoleVto>>> GetUserRoles([FromBody]Para<PermUserRoleVto> para)
        {
            var strWhere = string.Empty;
            para.SQL = @" SELECT u.UserName,u.`Name`,urs.RIds,urs.Descriptions 
FROM UserInfo u
LEFT JOIN 
(
	SELECT ur.UserName,GROUP_CONCAT(r.Id) AS RIds,GROUP_CONCAT(r.Description) AS Descriptions FROM PermUserRole ur 
	LEFT JOIN PermRole r ON ur.RId = r.Id
	WHERE ur.IsDel = 0
	AND r.IsDel = 0
	GROUP BY ur.UserName
) AS urs on urs.UserName = u.UserName
WHERE u.IsDel = 0 
{0} ";

            var query = para.Entity;
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.UserName))
                {
                    para.Filter = para.Filter.And(x => x.UserName.Contains(query.UserName));
                }
            }

            // 超级管理员
            if (!currentUser.IsHasRole(EnumSysRoles.SuperAdmin))
            {
                strWhere += $" AND NOT EXISTS (SELECT 1 FROM PermUserRole ur WHERE ur.UserName = u.UserName AND ur.RId = {(int)EnumSysRoles.SuperAdmin}) ";
            }

            para.SQL = string.Format(para.SQL, strWhere);
            para.OrderKey = " UserName Asc ";
            var pageRes = await works.QueryPageAsync(para);
            foreach (var item in pageRes.Datas)
            {
                item.LstRIds = item.RIds.StrSplitList().Select(x => Convert.ToInt32(x)).ToList();
            }

            return new Result<Pager<PermUserRoleVto>> { IsSucc = true, Message = "查询用户角色列表成功！", Data = pageRes };
        }

        /// <summary>
        /// 保存用户角色
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveUserRole")]
        public async Task<Result> SaveUserRole([FromBody]PermUserRoleVto item)
        {
            var userName = currentUser.UserName;
            var res = await works.WorkTransactionAsync(async () =>
            {
                // 根据用户查询已有角色
                var lstOld = await works.QueryAsync<PermUserRoleDto>(x => x.UserName == item.UserName);
                if (!(item.LstRIds != null && item.LstRIds.Count > 0))
                {
                    // 禁用已有角色
                    var disRes = await works.ExecuteCommandAsync(" UPDATE PermUserRole SET IsDel = 1,UpdateTime = CURRENT_TIMESTAMP,Updater = @Updater WHERE IsDel = 0 AND UserName = @UserName ",
                        new { UserName = item.UserName, Updater = userName });
                    return new Result { IsSucc = true };
                }

                // todo:sqlsugar 不支持批量Save 多主键的表，只能分开禁用、启用、新增
                // 禁用角色
                var lstNeedDisIds = lstOld.Where(x => x.IsDel == 0).Select(x => x.RId).Except(item.LstRIds).Distinct().ToList();
                if (lstNeedDisIds.Count > 0)
                {
                    var disRes = await works.ExecuteCommandAsync(
                        " UPDATE PermUserRole SET IsDel = 1,UpdateTime = CURRENT_TIMESTAMP,Updater = @Updater WHERE IsDel = 0 AND UserName = @UserName AND RId IN (@RIds) ",
                        new { UserName = item.UserName, Updater = userName, RIds = lstNeedDisIds });
                }

                // 更新角色
                var lstNeedUpdIds = lstOld.Where(x => x.IsDel == 1).Select(x => x.RId).Intersect(item.LstRIds).Distinct().ToList();
                if (lstNeedUpdIds.Count > 0)
                {
                    var updRes = await works.ExecuteCommandAsync(
                        " UPDATE PermUserRole SET IsDel = 0,UpdateTime = CURRENT_TIMESTAMP,Updater = @Updater WHERE IsDel = 1 AND UserName = @UserName AND RId IN (@RIds) ",
                        new { UserName = item.UserName, Updater = userName, RIds = lstNeedUpdIds });
                }

                // 新增角色
                var lstNewIds = item.LstRIds.Except(lstOld.Select(x => x.RId)).Distinct().ToList();
                if (lstNewIds.Count > 0)
                {
                    var dtNow = DateTime.Now;
                    var user = userName;
                    var lstSave = new List<PermUserRoleDto> { };
                    foreach (var rid in lstNewIds)
                    {
                        var temp = new PermUserRoleDto
                        {
                            UserName = item.UserName,
                            RId = rid,
                            Creater = user,
                            Updater = user,
                            IsDel = 0,
                            CreateTime = dtNow,
                            UpdateTime = dtNow
                        };

                        lstSave.Add(temp);
                    }

                    var addRes = await works.AddAsync(lstSave);
                }

                return new Result { IsSucc = true };
            });

            return new Result { IsSucc = res.IsSucc, Message = $"保存用户角色{(res.IsSucc ? "成功！" : "失败！")}" };
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("FilterUser")]
        public Result<dynamic> FilterUser([FromQuery]string name = "")
        {
            var lstUsers = cacheHelper.GetUserInfoCache();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                lstUsers = lstUsers.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Contains(name))
                    .Take(20).ToList();
            }

            var lstRes = lstUsers.Select(x => new { value = x.UserName, label = $"{x.UserName}({x.UserName})" }).Take(20).ToList();
            return new Result<dynamic> { IsSucc = true, Data = lstRes };
        }

        #endregion

        #region 角色-菜单管理

        /// <summary>
        /// 根据角色查询菜单Ids
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("GetMenusByRole")]
        public async Task<Result<List<int>>> GetMenusByRole([FromBody]JObject obj)
        {
            int rId = Convert.ToInt32(obj["rId"]);
            var lstRoleMenu = await works.QueryAsync<PermRoleMenuDto>(x => x.IsDel == 0 && x.RId == rId);
            var lstRoleMenuIds = lstRoleMenu.Select(x => x.MId).Distinct().ToList();
            return new Result<List<int>> { IsSucc = true, Data = lstRoleMenuIds };
        }

        /// <summary>
        /// 保存角色 对应 菜单Ids
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("SaveRoleMenus")]
        public async Task<Result> SaveRoleMenus([FromBody]JObject obj)
        {
            int rId = Convert.ToInt32(obj["rId"]);
            List<int> lstMIds = new List<int> { };
            if (obj["lstMIds"] != null)
            {
                lstMIds = ComHelper.JsonDeserialize<List<int>>(ComHelper.JsonSerialize(obj["lstMIds"]));
            }

            var dtNow = DateTime.Now;
            var optUser = currentUser.UserName;
            var res = await works.WorkTransactionAsync(async () =>
            {
                // 角色原有菜单
                var lstOld = await works.QueryAsync<PermRoleMenuDto>(x => x.RId == rId);

                // 禁用已有菜单
                if (!(lstMIds != null && lstMIds.Count > 0))
                {
                    var disRes = await works.ExecuteCommandAsync(" UPDATE PermRoleMenu SET IsDel = 1,UpdateTime = CURRENT_TIMESTAMP,Updater = @Updater WHERE IsDel = 0 AND RId = @RId ",
                        new { RId = rId, Updater = optUser });
                    return new Result { IsSucc = true };
                }

                // 需要删除的菜单Ids
                var lstDelIds = lstOld.Where(x => x.IsDel == 0).Select(x => x.MId).Except(lstMIds).Distinct().ToList();
                if (lstDelIds.Count > 0)
                {
                    var delRes = works.ExecuteCommand(@" UPDATE PermRoleMenu SET IsDel = 1,Updater = @Updater,UpdateTime = CURRENT_TIMESTAMP WHERE IsDel = 0 AND RId = @RId AND MId IN (@MIds) ",
                        new { RId = rId, Updater = optUser, MIds = lstDelIds });
                }

                // 需要更新的菜单Ids
                var lstUpdIds = lstOld.Where(x => x.IsDel == 1).Select(x => x.MId).Intersect(lstMIds).Distinct().ToList();
                if (lstUpdIds.Count > 0)
                {
                    var updRes = await works.ExecuteCommandAsync(
                       " UPDATE PermRoleMenu SET IsDel = 0,UpdateTime = CURRENT_TIMESTAMP,Updater = @Updater WHERE IsDel = 1 AND RId = @RId AND MId IN (@MIds) ",
                       new { RId = rId, Updater = optUser, MIds = lstUpdIds });
                }

                // 新增菜单Ids
                var lstNewIds = lstMIds.Except(lstOld.Select(x => x.MId)).Distinct().ToList();
                if (lstNewIds.Count > 0)
                {
                    var dtNow = DateTime.Now;
                    var user = optUser;
                    var lstSave = new List<PermRoleMenuDto> { };
                    foreach (var mId in lstNewIds)
                    {
                        var temp = new PermRoleMenuDto
                        {
                            RId = rId,
                            MId = mId,
                            Creater = user,
                            Updater = user,
                            IsDel = 0,
                            CreateTime = dtNow,
                            UpdateTime = dtNow
                        };

                        lstSave.Add(temp);
                    }

                    var addRes = await works.AddAsync(lstSave);
                }

                return new Result { IsSucc = true };
            });

            return new Result { IsSucc = res.IsSucc, Message = $"保存角色-菜单{(res.IsSucc ? "成功！" : "失败！")}" };
        }

        #endregion

        #region 初始化数据库

        /// <summary>
        /// SeedData
        /// </summary>
        /// <returns></returns>
        [HttpGet("SeedData")]
        [AllowAnonymous]
        public async Task<Result> SeedData()
        {
            try
            {
                await sugarDbSeed.SeedAsync();
                return new Result { IsSucc = true, Message = "还原数据库成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error($"SeedData：还原数据库失败:{ex.Message}", ex);
            }

            return new Result { Message = "还原数据库失败！" };
        }

        #endregion
    }
}