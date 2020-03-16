using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Rami.WebApi.Core.Service.Helpers;
using Rami.WebApi.Core.Web.Code;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 公众号API2
    /// </summary>
    [ApiAuthorize(AuthenticationSchemes = AuthHelper.JwtAuthScheme, Policy = AuthConst.ApiAuthSimple)]
    public class WxFunc2Controller : BaseController
    {
        /// <summary>
        /// 数据服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxFunc2Controller> logHelper;

        /// <summary>
        /// 当前用户
        /// </summary>
        private readonly CurrentUser currentUser;

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly ICacheHelper cacheHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="logHelper"></param>
        /// <param name="currentUser"></param>
        /// <param name="cacheHelper"></param>
        public WxFunc2Controller(IRepository repo, ILogHelper<WxFunc2Controller> logHelper, CurrentUser currentUser, ICacheHelper cacheHelper)
        {
            this.repo = repo;
            this.logHelper = logHelper;
            this.currentUser = currentUser;
            this.cacheHelper = cacheHelper;
        }

        #region 微信菜单管理

        /// <summary>
        /// 获取微信公众号菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMenus")]
        public async Task<Result<List<WxMenusShow>>> GetMenus()
        {
            try
            {
                var lstMenus = await repo.QueryAsync<WxMenusShow>(x => x.IsDel == 0);
                var lstLv1 = lstMenus.Where(x => x.Level == 1).ToList();
                foreach (var lv1 in lstLv1)
                {
                    lv1.SubButton = lstMenus.Where(x => x.Level == 2 && x.ParentId == lv1.Id).ToList();
                }

                return new Result<List<WxMenusShow>> { IsSucc = true, Data = lstLv1 };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetMenus:获取微信公众号菜单失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<List<WxMenusShow>> { Message = "获取微信公众号菜单失败！" };
        }

        /// <summary>
        /// 保存微信公众号菜单列表
        /// </summary>
        /// <param name="objPara"></param>
        /// <returns></returns>
        [HttpPost("SaveMenus")]
        public async Task<Result<string>> SaveMenus([FromBody]JObject objPara)
        {
            try
            {
                dynamic obj = objPara as dynamic;
                List<WxMenusShow> lstMenu = ComHelper.JsonDeserialize<List<WxMenusShow>>(obj.lstMenu.ToString());
                logHelper.Debug("SaveMenus:获取到待保存的公众号菜单数据：" + lstMenu.JsonSerialize());

                #region 保存菜单到公众号

                var wxMenus = new PubMenus();
                wxMenus.button = new List<PubMenu>();
                foreach (var lv1 in lstMenu)
                {
                    // 一级菜单
                    var wxMenuLv1 = new PubMenu();
                    wxMenuLv1.type = lv1.Type;
                    wxMenuLv1.name = lv1.Name;
                    wxMenuLv1.key = lv1.Key;
                    wxMenuLv1.url = lv1.Url;
                    wxMenuLv1.media_id = lv1.MediaId;
                    wxMenuLv1.appid = lv1.Appid;
                    wxMenuLv1.pagepath = lv1.Pagepath;
                    if (lv1.SubButton != null && lv1.SubButton.Count > 0)
                    {
                        wxMenuLv1.sub_button = new List<PubMenu>();
                        foreach (var lv2 in lv1.SubButton)
                        {
                            // 二级菜单
                            var wxMenuLv2 = new PubMenu();
                            wxMenuLv2.type = lv2.Type;
                            wxMenuLv2.name = lv2.Name;
                            wxMenuLv2.key = lv2.Key;
                            wxMenuLv2.url = lv2.Url;
                            wxMenuLv2.media_id = lv2.MediaId;
                            wxMenuLv2.appid = lv2.Appid;
                            wxMenuLv2.pagepath = lv2.Pagepath;
                            wxMenuLv1.sub_button.Add(wxMenuLv2);
                        }
                    }

                    wxMenus.button.Add(wxMenuLv1);
                }

                logHelper.Debug("SaveMenus:获取到待提交公众号的菜单数据：" + wxMenus.JsonSerialize());
                var apiRes = PubMenuApi.SaveMenu(wxMenus);
                logHelper.Debug("SaveMenus:保存公众号菜单结果：" + ComHelper.JsonSerialize(apiRes));
                if (!apiRes.IsSuss)
                {
                    var msg = "保存公众号菜单失败：错误代码:" + apiRes.errcode + "        原因：" + apiRes.errmsg;
                    logHelper.Error("SaveMenus:" + msg);
                    return new Result<string> { Message = msg };
                }

                #endregion

                #region 保存菜单到数据库

                var dtNow = DateTime.Now;
                var lstIds = new List<int>();
                var order = 1;
                var curUser = currentUser.UserName;
                foreach (var lv1 in lstMenu)
                {
                    lv1.Level = 1;
                    lv1.Order = order;
                    if (!lv1.CreateTime.HasValue)
                    {
                        lv1.Creater = curUser;
                        lv1.CreateTime = dtNow;
                    }

                    lv1.Updater = curUser;
                    lv1.UpdateTime = dtNow;
                    await repo.SaveAsync(lv1);
                    lstIds.Add(lv1.Id);
                    order++;

                    if (lv1.SubButton != null && lv1.SubButton.Count > 0)
                    {
                        lv1.Type = "";
                        lv1.Key = "";
                        lv1.Url = "";
                        lv1.MediaId = "";
                        lv1.Appid = "";
                        lv1.Pagepath = "";

                        foreach (var lv2 in lv1.SubButton)
                        {
                            lv2.ParentId = lv1.Id;
                            lv2.Level = 2;
                            lv2.Order = order;
                            if (!lv2.CreateTime.HasValue)
                            {
                                lv2.Creater = curUser;
                                lv2.CreateTime = dtNow;
                            }

                            lv2.Updater = curUser;
                            lv2.UpdateTime = dtNow;
                            await repo.SaveAsync(lv2);
                            lstIds.Add(lv2.Id);
                            order++;
                        }
                    }
                }

                // 禁用掉停用的菜单
                if (lstIds.Count > 0)
                {
                    var strIds = string.Join(",", lstIds);
                    var strSql = string.Format(" UPDATE WxMenus SET IsDel = 1, Updater = '{0}', UpdateTime = '{1}'  WHERE Id NOT IN ({2}) ", currentUser.UserName, dtNow, strIds);
                    await repo.ExecuteCommandAsync(strSql);
                }

                #endregion

                return new Result<string> { IsSucc = true, Message = "保存微信公众号菜单列表成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetMenus:保存微信公众号菜单列表失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "保存微信公众号菜单列表失败！" };
        }

        /// <summary>
        /// 同步微信菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("SyncMenus")]
        public async Task<Result<string>> SyncMenus()
        {
            try
            {
                var apiRes = PubMenuApi.GetMenu();
                logHelper.Debug("SyncMenus:同步微信菜单结果：" + apiRes.JsonSerialize());
                if (!apiRes.IsSuss)
                {
                    var msg = "同步微信菜单结果失败：错误代码:" + apiRes.errcode + "        原因：" + apiRes.errmsg;
                    logHelper.Error("SaveMenus:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 保存菜单到数据库
                var lstMenus = apiRes.menu.button;
                var order = 1;
                var dtNow = DateTime.Now;
                var lstIds = new List<int>();
                var curUser = currentUser.UserName;
                for (int i = 0; i < lstMenus.Count; i++)
                {
                    var lv1 = lstMenus[i];
                    // 一级菜单
                    var lv1Menu = new WxMenusShow();
                    lv1Menu.Level = 1;
                    lv1Menu.Order = order;
                    lv1Menu.Name = lv1.name;
                    lv1Menu.Type = lv1.type;
                    lv1Menu.Key = lv1.key;
                    lv1Menu.Url = lv1.url;
                    lv1Menu.MediaId = lv1.media_id;
                    lv1Menu.Appid = lv1.appid;
                    lv1Menu.Pagepath = lv1.pagepath;
                    lv1Menu.Creater = curUser;
                    lv1Menu.Updater = curUser;
                    lv1Menu.CreateTime = dtNow;
                    lv1Menu.UpdateTime = dtNow;
                    await repo.SaveAsync(lv1Menu);
                    lstIds.Add(lv1Menu.Id);
                    order++;

                    for (int j = 0; j < lv1.sub_button.Count; j++)
                    {
                        var lv2 = lv1.sub_button[j];
                        // 二级菜单
                        var lv2Menu = new WxMenusShow();
                        lv2Menu.ParentId = lv1Menu.Id;
                        lv2Menu.Level = 2;
                        lv2Menu.Order = order;
                        lv2Menu.Name = lv2.name;
                        lv2Menu.Type = lv2.type;
                        lv2Menu.Key = lv2.key;
                        lv2Menu.Url = lv2.url;
                        lv2Menu.MediaId = lv2.media_id;
                        lv2Menu.Appid = lv2.appid;
                        lv2Menu.Pagepath = lv2.pagepath;
                        lv2Menu.Creater = curUser;
                        lv2Menu.Updater = curUser;
                        lv2Menu.CreateTime = dtNow;
                        lv2Menu.UpdateTime = dtNow;
                        await repo.SaveAsync(lv2Menu);
                        lstIds.Add(lv2Menu.Id);
                        order++;
                    }
                }

                // 禁用掉停用的菜单
                if (lstIds.Count > 0)
                {
                    var strIds = string.Join(",", lstIds);
                    var strSql = string.Format(" UPDATE WxMenus SET IsDel = 1, Updater = '{0}', UpdateTime = '{1}'  WHERE Id NOT IN ({2}) ", currentUser.UserName, dtNow, strIds);
                    await repo.ExecuteCommandAsync(strSql);
                }

                return new Result<string> { IsSucc = true, Message = "同步微信菜单结果成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SyncMenus:同步微信菜单结果失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "同步微信菜单结果失败！" };
        }

        #endregion

        #region 微信客服

        /// <summary>
        /// 获取微信客服列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetWxKfList")]
        public async Task<Result<Pager<WxKfInfoShow>>> GetWxKfList([FromBody]Para<WxKfInfoShow> para)
        {
            try
            {
                var query = para.Entity;
                if (query != null)
                {
                    if (!string.IsNullOrEmpty(query.KfAccount))
                    {
                        para.Filter = para.Filter.And(x => x.KfAccount.Contains(query.KfAccount));
                    }

                    if (!string.IsNullOrEmpty(query.KfNick))
                    {
                        para.Filter = para.Filter.And(x => x.KfNick.Contains(query.KfNick));
                    }

                    if (!string.IsNullOrEmpty(query.KfWx))
                    {
                        para.Filter = para.Filter.And(x => x.KfWx.Contains(query.KfWx));
                    }
                }

                para.Filter = para.Filter.And(x => x.IsDel == 0);
                para.OrderKey = " UpdateTime Desc ";
                var pageRes = await repo.QueryPageAsync(para);
                // 头像
                var root = ComHelper.GetAbsPath("~/");
                foreach (var item in pageRes.Datas)
                {
                    item.KfHeadUpShow = ComHelper.UpdImgAbsPath(root, item.KfHeadUpVir);
                }

                return new Result<Pager<WxKfInfoShow>> { IsSucc = true, Data = pageRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetWxKfList:获取微信客服列表失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<Pager<WxKfInfoShow>> { Message = "获取微信客服列表失败！" };
        }

        /// <summary>
        /// 更新微信客服信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveWxKf")]
        public async Task<Result<string>> SaveWxKf([FromBody]WxKfInfoShow item)
        {
            try
            {
                var dtNow = DateTime.Now;
                var userName = currentUser.UserName;
                // 客服信息
                var kfInfo = new PubKf();
                // 客服账号名称
                var kfSuffix = cacheHelper.GetDbConfValByKey("kfsuffix");
                var accName = item.KfAccount;
                if (!accName.EndsWith(kfSuffix))
                {
                    item.KfAccount = string.Format("{0}@{1}", accName, kfSuffix);
                }

                kfInfo.kf_account = item.KfAccount;
                kfInfo.nickname = item.KfNick;
                kfInfo.password = ComHelper.MD5Sign("ramikf");
                if (!item.CreateTime.HasValue)
                {
                    // 创建客服
                    var addRes = PubKfApi.AddKF(kfInfo);
                    logHelper.Debug("SaveWxKf:创建客服结果:" + addRes.JsonSerialize());
                    if (!addRes.IsSuss)
                    {
                        var msg = "创建客服失败:错误代码：" + addRes.errcode + "     信息：" + addRes.errmsg;
                        logHelper.Error("SaveWxKf:" + msg);
                        return new Result<string> { Message = msg };
                    }

                    // 保存客服信息
                    item.Creater = userName;
                    item.Updater = userName;
                    item.CreateTime = dtNow;
                    item.UpdateTime = dtNow;
                    await repo.AddAsync(item);

                    return new Result<string> { IsSucc = true, Message = "创建客服成功！" };
                }
                else
                {
                    // 修改客服
                    var editRes = PubKfApi.UpdateKF(kfInfo);
                    logHelper.Debug("SaveWxKf:更新客服结果:" + editRes.JsonSerialize());
                    if (!editRes.IsSuss)
                    {
                        var msg = "更新客服失败:错误代码：" + editRes.errcode + "     信息：" + editRes.errmsg;
                        logHelper.Error("SaveWxKf:" + msg);
                        return new Result<string> { Message = msg };
                    }

                    // 保存客服信息
                    item.Updater = userName;
                    item.UpdateTime = dtNow;
                    var upRes = repo.Update(item, new List<string> { "KfNick", "Updater", "UpdateTime" });

                    return new Result<string> { IsSucc = upRes, Message = $"更新客服{(upRes ? "成功！" : "失败！")}" };
                }
            }
            catch (Exception ex)
            {
                logHelper.Error("SaveWxKf:更新微信客服信息失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "更新微信客服信息失败！" };
        }

        /// <summary>
        /// 更新微信客服头像
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("UpdWxKfImg")]
        public async Task<Result<string>> UpdWxKfImg([FromBody]WxKfInfoShow item)
        {
            try
            {
                // 转换路径
                if (string.IsNullOrEmpty(item.KfHeadUpVir))
                {
                    return new Result<string> { Message = "请先上传客服头像！" };
                }

                // 头像物理路径
                var phyPath = ComHelper.GetPhyWWWRoot(item.KfHeadUpVir);
                var apiRes = PubKfApi.SetKFHeadImg(phyPath, item.KfAccount);
                logHelper.Debug("UpdWxKfImg:修改客服头像结果:" + apiRes.JsonSerialize());
                if (!apiRes.IsSuss)
                {
                    var msg = "修改客服头像:错误代码：" + apiRes.errcode + "     信息：" + apiRes.errmsg;
                    logHelper.Error("UpdWxKfImg:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 保存客服信息
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                var upRes = await repo.UpdateAsync(item, new List<string> { "KfHeadUpVir", "Updater", "UpdateTime" });

                return new Result<string> { IsSucc = upRes, Message = $"更新微信客服头像{(upRes ? "成功！" : "失败！")}" };
            }
            catch (Exception ex)
            {
                logHelper.Error("UpdWxKfImg:更新微信客服头像失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "更新微信客服头像失败！" };
        }

        /// <summary>
        /// 邀请微信客服
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("InviteWxKf")]
        public async Task<Result<string>> InviteWxKf([FromBody]WxKfInfoShow item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.InviteWx))
                {
                    return new Result<string> { Message = "请先输入客服微信号！" };
                }

                var apiRes = PubKfApi.SendKfInvite(item.KfAccount, item.InviteWx);
                logHelper.Debug("InviteWxKf:邀请微信客服结果:" + apiRes.JsonSerialize());
                if (!apiRes.IsSuss)
                {
                    var msg = "邀请微信客服:错误代码：" + apiRes.errcode + "     信息：" + apiRes.errmsg;
                    logHelper.Error("InviteWxKf:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 保存客服信息
                item.InviteWx = item.InviteWx;
                item.InviteStatus = "waiting";
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                var upRes = await repo.UpdateAsync(item, new List<string> { "InviteWx", "InviteStatus", "Updater", "UpdateTime" });

                return new Result<string> { IsSucc = upRes, Message = $"邀请微信客服{(upRes ? "成功！" : "失败！")}" };
            }
            catch (Exception ex)
            {
                logHelper.Error("InviteWxKf:邀请微信客服失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "邀请微信客服失败！" };
        }

        /// <summary>
        /// 删除微信客服
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("DelWxKfImg")]
        public async Task<Result<string>> DelWxKfImg([FromBody]WxKfInfoShow item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.KfAccount))
                {
                    return new Result<string> { Message = "请先输入客服账号！" };
                }

                var apiRes = PubKfApi.DeleteKF(item.KfAccount);
                logHelper.Debug("DelWxKfImg:删除微信客服结果:" + apiRes.JsonSerialize());
                if (!apiRes.IsSuss)
                {
                    var msg = "删除微信客服:错误代码：" + apiRes.errcode + "     信息：" + apiRes.errmsg;
                    logHelper.Error("DelWxKfImg:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 保存客服信息
                item.IsDel = 1;
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                var upRes = await repo.UpdateAsync(item, new List<string> { "IsDel", "Updater", "UpdateTime" });

                return new Result<string> { IsSucc = upRes, Message = $"删除微信客服{(upRes ? "成功！" : "失败！")}" };
            }
            catch (Exception ex)
            {
                logHelper.Error("DelWxKfImg:删除微信客服失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "删除微信客服失败！" };
        }

        /// <summary>
        /// 同步微信客服信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SyncWxKf")]
        public async Task<Result<string>> SyncWxKf([FromBody]WxKfInfoShow item)
        {
            try
            {
                var apiRes = PubKfApi.GetKFList();
                logHelper.Debug("SyncWxKf:同步微信客服信息结果:" + apiRes.JsonSerialize());
                if (!apiRes.IsSuss)
                {
                    var msg = "同步微信客服信息:错误代码：" + apiRes.errcode + "     信息：" + apiRes.errmsg;
                    logHelper.Error("SyncWxKf:" + msg);
                    return new Result<string> { Message = msg };
                }

                var lstKf = apiRes.kf_list;
                // 保存客服列表信息
                var dtNow = DateTime.Now;
                var userName = currentUser.UserName;
                var lstKfAcounts = new List<string>();
                foreach (var kf in lstKf)
                {
                    var exist = await repo.FirstOrDefaultAsync<WxKfInfoShow>(x => x.KfAccount == kf.kf_account);
                    if (exist == null)
                    {
                        exist = new WxKfInfoShow();
                        exist.Creater = userName;
                        exist.CreateTime = dtNow;
                    }

                    exist.KfAccount = kf.kf_account;
                    exist.KfNick = kf.kf_nick;
                    exist.KfId = kf.kf_id;
                    exist.KfHeadimgurl = kf.kf_headimgurl;
                    exist.KfWx = kf.kf_wx;
                    exist.InviteWx = kf.invite_wx;
                    exist.InviteExpireTime = kf.invite_expire_time;
                    exist.InviteStatus = kf.invite_status;
                    exist.IsDel = 0;
                    exist.Updater = userName;
                    exist.UpdateTime = dtNow;
                    var saveRes = await repo.SaveAsync(exist);
                    lstKfAcounts.Add(exist.KfAccount);
                }

                // 删除不用的客服
                if (lstKfAcounts.Count > 0)
                {
                    var delRes = await repo.ExecuteCommandAsync(" UPDATE WxKfInfo SET IsDel = 1, Updater = @userName, UpdateTime = @dtNow WHERE KfAccount NOT IN (@lstKfAcounts) ",
                       new { userName, dtNow, lstKfAcounts });
                }

                return new Result<string> { IsSucc = true, Message = "同步微信客服信息成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SyncWxKf:同步微信客服信息失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "同步微信客服信息失败！" };
        }

        #endregion

        #region 后台图文管理

        /// <summary>
        /// 获取后台图文列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetBackNews")]
        public async Task<Result<Pager<WxBackNewsShow>>> GetBackNews([FromBody]Para<WxBackNewsShow> para)
        {
            try
            {
                var query = para.Entity;
                if (query != null)
                {
                    if (!string.IsNullOrEmpty(query.Name))
                    {
                        para.Filter = para.Filter.And(x => x.Name.Contains(query.Name));
                    }
                }

                para.OrderKey = " IsDel ASC,UpdateTime Desc ";
                var pageRes = await repo.QueryPageAsync(para);
                UpdBackNewsInfo(pageRes.Datas);

                return new Result<Pager<WxBackNewsShow>> { IsSucc = true, Data = pageRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetBackNews:获取后台图文列表失败：" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<Pager<WxBackNewsShow>> { Message = "获取后台图文列表失败！" };
        }

        /// <summary>
        /// 更新图文信息
        /// </summary>
        /// <param name="lstDatas"></param>
        private void UpdBackNewsInfo(List<WxBackNewsShow> lstDatas)
        {
            var root = ComHelper.GetAbsPath("~/");
            foreach (var item in lstDatas)
            {
                // 路径
                item.ArticleUrlShow = item.ArticleUrl;
                if (!string.IsNullOrEmpty(item.ArticleUrl) && item.ArticleUrl.StartsWith("~/"))
                {
                    item.ArticleUrlShow = ComHelper.UpdImgAbsPath(root, item.ArticleUrl);
                }

                // 图片
                if (!string.IsNullOrEmpty(item.ImgUrlVir))
                {
                    item.ImgUrlShow = ComHelper.UpdImgAbsPath(root, item.ImgUrlVir);
                }
            }
        }

        /// <summary>
        /// 更新后台图文状态
        /// </summary>
        /// <param name="objPara"></param>
        /// <returns></returns>
        [HttpPost("UpdBackNews")]
        public async Task<Result<string>> UpdBackNews([FromBody]JObject objPara)
        {
            try
            {
                int id = Convert.ToInt32(objPara["Id"]);
                var item = repo.FirstOrDefault<WxBackNewsShow>(x => x.Id == id);
                item.IsDel = 1 - item.IsDel;
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                var upRes = await repo.UpdateAsync(item, new List<string> { "IsDel", "Updater", "UpdateTime" });
                return new Result<string> { IsSucc = upRes, Message = $"更新后台图文状态{(upRes ? "成功！" : "失败!")}" };
            }
            catch (Exception ex)
            {
                logHelper.Error("UpdBackNews:更新后台图文状态失败：" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<string> { Message = "更新后台图文状态失败！" };
        }

        /// <summary>
        /// 根据Id获取后台图文
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetBackNewsById")]
        public async Task<Result<WxBackNewsShow>> GetBackNewsById([FromQuery]long id)
        {
            try
            {
                var item = await repo.FirstOrDefaultAsync<WxBackNewsShow>(x => x.Id == id);
                UpdBackNewsInfo(new List<WxBackNewsShow> { item });
                item.UpFile = UploadFileHelper.GetUploadFileInfo(item.ImgUrlVir);
                return new Result<WxBackNewsShow> { IsSucc = true, Data = item };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetBackNewsById:获取后台图文失败：" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<WxBackNewsShow> { Message = "获取后台图文失败！" };
        }

        /// <summary>
        /// 保存后台图文列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveBackNews")]
        public async Task<Result<string>> SaveBackNews([FromBody]WxBackNewsShow item)
        {
            try
            {
                var userName = currentUser.UserName;
                var dtNow = DateTime.Now;
                item.Updater = userName;
                item.UpdateTime = dtNow;
                if (!item.CreateTime.HasValue)
                {
                    item.Creater = userName;
                    item.CreateTime = dtNow;
                }

                var saveRes = await repo.SaveAsync(item);
                // 文章链接
                item.ArticleUrl = "~/wxfunc/backnewsshow?id=" + item.Id;
                repo.Update(item, "ArticleUrl");
                return new Result<string> { IsSucc = true, Message = "保存后台图文成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SaveBackNews:保存后台图文失败：" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<string> { Message = "保存后台图文失败！" };
        }

        #endregion
    }
}