using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Web.Code;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 公众号API
    /// </summary>
    [ApiAuthorize(AuthenticationSchemes = AuthHelper.JwtAuthScheme, Policy = AuthConst.ApiAuthSimple)]
    public class WxFuncController : BaseController
    {
        /// <summary>
        /// 数据服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxFuncController> logHelper;

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
        public WxFuncController(IRepository repo, ILogHelper<WxFuncController> logHelper, CurrentUser currentUser)
        {
            this.repo = repo;
            this.logHelper = logHelper;
            this.currentUser = currentUser;
        }

        #region 永久素材管理

        /// <summary>
        /// 获取媒体列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetMediaList")]
        public async Task<Result<Pager<WxMediaShow>>> GetMediaList([FromBody]Para<WxMediaShow> para)
        {
            try
            {
                if (para.Entity != null)
                {
                    var item = para.Entity;
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        para.Filter = para.Filter.And(x => x.Title.Contains(item.Title));
                    }
                    if (!string.IsNullOrEmpty(item.Type))
                    {
                        para.Filter = para.Filter.And(x => x.Type == item.Type);
                    }
                }

                para.Filter = para.Filter.And(x => x.IsDel == 0);
                para.OrderKey = " UpdateTime Desc ";
                var pageRes = await repo.QueryPageAsync(para);
                // 处理本地路径
                UpdMediaInfo(pageRes.Datas);

                return new Result<Pager<WxMediaShow>> { IsSucc = true, Data = pageRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetMediaList:获取永久素材列表失败" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<Pager<WxMediaShow>> { Message = "获取永久素材列表失败!" };
        }

        /// <summary>
        /// 更新素材信息
        /// </summary>
        /// <param name="lstData"></param>
        private void UpdMediaInfo(List<WxMediaShow> lstData)
        {
            var root = ComHelper.GetAbsPath("~/");
            foreach (var item in lstData)
            {
                item.LocalUrlShow = ComHelper.UpdImgAbsPath(root, item.LocalUrl);
                if (!string.IsNullOrEmpty(item.Type))
                {
                    item.TypeShow = ComHelper.GetEnumValueByStr<PubMediaType>(item.Type).ToString();
                }
            }
        }

        /// <summary>
        /// 保存永久素材
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveMedia")]
        public Result<string> SaveMedia([FromBody]WxMediaShow item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Type) || string.IsNullOrEmpty(item.LocalUrl))
                {
                    return new Result<string> { Message = "保存失败：参数不足！" };
                }

                var mType = ComHelper.GetEnumValueByStr<PubMediaType>(item.Type);
                var filePath = ComHelper.GetPhyWWWRoot(item.LocalUrl);

                // 上传永久素材
                var upRes = PubMediaApi.AddMaterial(mType, filePath, item.Title, item.Introduction);
                logHelper.Debug("SaveMedia:上传微信永久素材结果:" + upRes.JsonSerialize());
                if (!upRes.IsSuss)
                {
                    var msg = "上传微信永久素材失败：" + upRes.errcode + "     " + upRes.errmsg;
                    logHelper.Debug("SaveMedia:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 保存上传结果
                var userName = currentUser.UserName;
                var dtNow = DateTime.Now;
                item.MediaId = upRes.media_id;
                item.WxUrl = upRes.url;
                item.Creater = userName;
                item.Updater = userName;
                item.CreateTime = dtNow;
                item.UpdateTime = dtNow;
                repo.Save(item);

                return new Result<string> { IsSucc = true, Message = "保存永久素材成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SaveMedia:保存永久素材失败：" + ex.Message + "      " + ex.StackTrace);
            }

            return new Result<string> { Message = "保存永久素材失败!" };
        }

        /// <summary>
        /// 更新永久素材名称
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("UpdMediaName")]
        public Result<string> UpdMediaName([FromBody]WxMediaShow item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Title))
                {
                    return new Result<string> { Message = "请输入永久素材的名称！" };
                }

                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                repo.Update(item, new List<string> { "Title", "Updater", "UpdateTime" });
                return new Result<string> { IsSucc = true, Message = "更新永久素材名称成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("UpdMediaName:更新永久素材名称失败：" + ex.Message + "      " + ex.StackTrace);
            }

            return new Result<string> { Message = "更新永久素材名称失败!" };
        }

        /// <summary>
        /// 删除永久素材
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("DelMedia")]
        public Result<string> DelMedia([FromBody]WxMediaShow item)
        {
            try
            {
                // 删除永久素材
                var delRes = PubMediaApi.DeleteMaterial(item.MediaId);
                logHelper.Debug("DelMedia:删除永久素材结果:" + delRes.JsonSerialize());
                if (!delRes.IsSuss)
                {
                    var msg = "删除永久素材失败：" + delRes.errcode + "     " + delRes.errmsg;
                    logHelper.Error("DelMedia:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 保存数据库记录
                item.IsDel = 1;
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                repo.Update(item, new List<string> { "IsDel", "Updater", "UpdateTime" });

                return new Result<string> { IsSucc = true, Message = "删除永久素材成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("DelMedia:删除永久素材失败：" + ex.Message + "      " + ex.StackTrace);
            }

            return new Result<string> { Message = "删除永久素材失败!" };
        }

        /// <summary>
        /// 同步微信公众号永久素材
        /// </summary>
        /// <returns></returns>
        [HttpPost("SyncMediaList")]
        public Result<string> SyncMediaList([FromBody]JObject objPara)
        {
            try
            {
                // 参数
                dynamic obj = objPara as dynamic;
                string type = obj.type.ToString();
                int count = Convert.ToInt32(obj.count.ToString());
                if (string.IsNullOrEmpty(type))
                {
                    return new Result<string> { Message = "请先选择要同步的素材类别！" };
                }

                if (count <= 0)
                {
                    return new Result<string> { Message = "请先选择要同步的素材的数量！" };
                }

                // 所有的素材
                var lstAll = new List<PubMeterial>();
                // 同步素材
                var mType = ComHelper.GetEnumValueByStr<PubMediaType>(type);
                var startInd = 0;
                var pageSize = 20;
                var syncRes = PubMediaApi.GetMaterialList(mType, startInd, pageSize);
                logHelper.Debug("SyncMediaList:同步微信公众号永久素材结果:" + syncRes.JsonSerialize());
                if (!syncRes.IsSuss)
                {
                    var msg = "同步微信公众号永久素材失败：" + syncRes.errcode + "     " + syncRes.errmsg;
                    logHelper.Error("SyncMediaList:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 总数
                var allCount = syncRes.total_count;
                if (allCount > count)
                {
                    allCount = count;
                }

                lstAll.AddRange(syncRes.item);
                SyncMedias(mType, syncRes);

                // 循环同步
                while (syncRes.IsSuss && lstAll.Count < allCount)
                {
                    startInd += pageSize;
                    syncRes = PubMediaApi.GetMaterialList(mType, startInd, pageSize);
                    logHelper.Debug("SyncMediaList:同步微信公众号永久素材结果:" + syncRes.JsonSerialize());
                    if (syncRes.IsSuss)
                    {
                        lstAll.AddRange(syncRes.item);
                        SyncMedias(mType, syncRes);
                    }
                }

                return new Result<string> { IsSucc = true, Message = "同步微信公众号永久素材成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SyncMediaList:同步微信公众号永久素材失败：" + ex.Message + "      " + ex.StackTrace);
            }

            return new Result<string> { Message = "同步微信公众号永久素材失败！" };
        }

        /// <summary>
        /// 保存同步的素材和下载
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="syncRes"></param>
        private void SyncMedias(PubMediaType mType, PubMeterialResult syncRes)
        {
            try
            {
                // 保存永久素材记录
                var dtNow = DateTime.Now;
                var lstWxMedia = new List<WxMediaShow>();
                var userName = currentUser.UserName;
                foreach (var media in syncRes.item)
                {
                    // 判断是否存在
                    var exist = repo.FirstOrDefault<WxMediaShow>(x => x.MediaId == media.media_id);
                    if (exist == null)
                    {
                        exist = new WxMediaShow();
                        exist.Creater = userName;
                        exist.CreateTime = dtNow;
                        exist.Title = media.name;
                    }

                    exist.MediaId = media.media_id;
                    exist.Type = mType.ToString();
                    exist.WxUrl = media.url;
                    exist.Updater = userName;
                    exist.UpdateTime = dtNow;
                    exist.IsDel = 0;
                    repo.Save(exist);
                    lstWxMedia.Add(exist);
                }

                // 下载附件
                var upVirPath = UpPathHelper.GetUploadVirPath("wxmedia", dtNow);
                var upPhyPath = UpPathHelper.GetUploadPhyPath("wxmedia", dtNow);
                DownWxMedia(lstWxMedia, mType, upVirPath, upPhyPath);
            }
            catch (Exception ex)
            {
                logHelper.Error("SyncMedias:保存永久素材失败：" + ex.Message + "     " + ex.StackTrace);
            }
        }

        /// <summary>
        /// 下载微信永久素材
        /// </summary>
        /// <param name="lstWxMedia"></param>
        /// <param name="mType"></param>
        /// <param name="virPath"></param>
        /// <param name="phyPath"></param>
        private void DownWxMedia(List<WxMediaShow> lstWxMedia, PubMediaType mType, string virPath, string phyPath)
        {
            var client = new WebClient();
            var dtNow = DateTime.Now;
            var userName = currentUser.UserName;
            foreach (var item in lstWxMedia)
            {
                string extension = GetMediaExtension(mType, item);
                var fileName = item.MediaId + extension;
                var filePhyPath = phyPath + fileName;
                // 判断路径是否存在
                var fi = new FileInfo(filePhyPath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }

                // 视频特殊处理
                if (mType == PubMediaType.video)
                {
                    var vRes = PubMediaApi.GetMaterialVedio(item.MediaId, mType);
                    logHelper.Debug("DownWxMedia:下载视频素材结果：" + vRes.JsonSerialize());
                    if (vRes.IsSuss)
                    {
                        item.WxUrl = vRes.down_url;
                        item.Title = vRes.title;
                        item.Introduction = vRes.description;
                        item.Updater = userName;
                        item.UpdateTime = dtNow;
                        repo.Update(item, new List<string> { "WxUrl", "Title", "Introduction", "Updater", "UpdateTime" });
                    }
                }

                // 下载微信永久素材
                if (!string.IsNullOrEmpty(item.WxUrl))
                {
                    // 有返回url链接的
                    client.DownloadFile(item.WxUrl, filePhyPath);
                }
                else
                {
                    // 其他素材直接下载
                    PubMediaApi.GetMaterial(item.MediaId, filePhyPath, mType);
                }

                // 更新永久素材路径
                item.LocalUrl = virPath + fileName;
                item.Updater = userName;
                item.UpdateTime = dtNow;
                repo.Update(item, new List<string> { "LocalUrl", "Updater", "UpdateTime" });
            }
        }

        /// <summary>
        /// 获取素材的默认格式
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetMediaExtension(PubMediaType mType, WxMediaShow item)
        {
            var extension = Path.GetExtension(item.Title);
            if (string.IsNullOrEmpty(extension))
            {
                switch (mType)
                {
                    case PubMediaType.image:
                        extension = ".jpg";
                        break;
                    case PubMediaType.voice:
                        extension = ".mp3";
                        break;
                    case PubMediaType.video:
                        extension = ".mp4";
                        break;
                    case PubMediaType.thumb:
                        extension = ".jpg";
                        break;
                    default:
                        break;
                }
            }

            return extension;
        }

        #endregion

        #region 永久图文管理

        /// <summary>
        /// 获取永久图文列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetNewsList")]
        public async Task<Result<Pager<WxNewsShow>>> GetNewsList([FromBody]Para<WxNewsShow> para)
        {
            try
            {
                var query = para.Entity;
                if (query != null)
                {
                    if (!string.IsNullOrEmpty(query.FirstNewsTitle))
                    {
                        para.Filter = para.Filter.And(x => x.FirstNewsTitle.Contains(query.FirstNewsTitle));
                    }
                }

                para.Filter = para.Filter.And(x => x.IsDel == 0);

                var pageRes = await repo.QueryPageAsync(para);
                return new Result<Pager<WxNewsShow>> { IsSucc = true, Data = pageRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetNewsList：获取微信永久图文列表失败：" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<Pager<WxNewsShow>> { Message = "获取微信永久图文列表失败！" };
        }

        /// <summary>
        /// 根据媒体ID获取图文列表
        /// </summary>
        /// <param name="mId"></param>
        /// <returns></returns>
        [HttpGet("GetWxNewsById")]
        public async Task<Result<List<WxNewsDetailShow>>> GetWxNewsById([FromQuery]string mId)
        {
            try
            {
                var lstDatas = await repo.QueryAsync<WxNewsDetailShow>(x => x.MediaId == mId && x.IsDel == 0);

                // 缩略图列表
                var dicThumb = new Dictionary<string, string> { };
                var lstThumbIds = lstDatas.Where(x => !string.IsNullOrWhiteSpace(x.ThumbMediaId)).Select(x => x.ThumbMediaId).ToList();
                if (lstThumbIds.Count > 0)
                {
                    var lstThumb = await repo.QueryAsync<WxMediaShow>(x => lstThumbIds.Contains(x.MediaId));
                    dicThumb = lstThumb.ToDictionary(x => x.MediaId, y => y.LocalUrl);
                }

                var root = ComHelper.GetAbsPath("~/");
                foreach (var item in lstDatas)
                {
                    // 预览路径
                    if (!string.IsNullOrEmpty(item.ContentSourceUrl))
                    {
                        if (item.ContentSourceUrl.StartsWith("~/"))
                        {
                            item.ContentSourceUrlShow = ComHelper.UpdImgAbsPath(root, item.ContentSourceUrl);
                        }
                        else
                        {
                            item.ContentSourceUrlShow = item.ContentSourceUrl;
                        }
                    }

                    // 缩略图
                    if (!string.IsNullOrWhiteSpace(item.ThumbMediaId) && dicThumb.ContainsKey(item.ThumbMediaId))
                    {
                        item.ThumbUrlShow = ComHelper.UpdImgAbsPath(root, dicThumb[item.ThumbMediaId]);
                    }
                }

                return new Result<List<WxNewsDetailShow>> { IsSucc = true, Data = lstDatas };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetWxNewsById:根据媒体ID获取图文列表失败:" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<List<WxNewsDetailShow>> { Message = "根据媒体ID获取图文列表失败！" };
        }

        /// <summary>
        /// 获取所有的永久图片素材
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetWxImgMedias")]
        public async Task<Result<List<WxMediaShow>>> GetWxImgMedias()
        {
            try
            {
                var lstImgs = await repo.QueryAsync<WxMediaShow>(x => x.Type == PubMediaType.image.ToString() || x.Type == PubMediaType.thumb.ToString());
                // 显示图片的路径
                var root = ComHelper.GetAbsPath("~/");
                foreach (var item in lstImgs)
                {
                    item.LocalUrlShow = ComHelper.UpdImgAbsPath(root, item.LocalUrl);
                }

                return new Result<List<WxMediaShow>> { IsSucc = true, Data = lstImgs };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetWxImgMedias:获取所有的永久图片素材失败:" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<List<WxMediaShow>> { Message = "获取所有的永久图片素材失败！" };
        }

        /// <summary>
        /// 保存微信图文素材
        /// </summary>
        /// <param name="objPara"></param>
        /// <returns></returns>
        [HttpPost("SaveWxNews")]
        public async Task<Result<string>> SaveWxNews([FromBody]JObject objPara)
        {
            try
            {
                dynamic obj = objPara as dynamic;
                var lstNews = ComHelper.JsonDeserialize<List<WxNewsDetailShow>>(obj.lstNews.ToString());
                var now = DateTime.Now;
                // 先把图文保存到数据
                var userName = currentUser.UserName;
                foreach (var item in lstNews)
                {
                    item.ShowCoverPic = 1;
                    item.IsDel = 1;
                    item.Creater = userName;
                    item.Updater = userName;
                    item.CreateTime = now;
                    item.UpdateTime = now;
                    //await repo.SaveAsync(item);
                }

                // 批量保存
                var mulSaveRes = await repo.SaveAsync(lstNews);

                // 构造微信图文消息
                var lstWxNew = new List<PubMediaArticle>();
                var root = ComHelper.GetAbsPath("~/");
                // TODO:预览界面待开发
                var newsVirUrl = "~/WxAdmin/Wechat/WxNewsShow?id=";
                foreach (var item in lstNews)
                {
                    // 文章相对路径
                    item.ContentSourceUrl = newsVirUrl + item.Id;
                    // 文章真实路径
                    item.ContentSourceUrlShow = ComHelper.UpdImgAbsPath(root, item.ContentSourceUrl);

                    // 保存图文信息
                    var temp = new PubMediaArticle();
                    temp.title = item.Title;
                    temp.thumb_media_id = item.ThumbMediaId;
                    temp.author = item.Author;
                    temp.digest = item.Digest;
                    temp.show_cover_pic = 1;
                    temp.content = item.Content;
                    temp.content_source_url = item.ContentSourceUrlShow;
                    lstWxNew.Add(temp);
                }

                // 创建永久图文消息
                var newRes = PubMediaApi.AddNews(lstWxNew);
                logHelper.Debug("SaveWxNews:保存图文消息结果:" + newRes.JsonSerialize());
                if (!newRes.IsSuss)
                {
                    var msg = "保存图文消息失败:代码：" + newRes.errcode + "   原因：" + newRes.errmsg;
                    logHelper.Error("SaveWxNews:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 更新图文信息
                foreach (var item in lstNews)
                {
                    item.MediaId = newRes.media_id;
                    item.IsDel = 0;
                    //await repo.SaveAsync(item);
                }

                // 批量保存
                mulSaveRes = await repo.SaveAsync(lstNews);

                // 保存图文消息
                var dtNow = DateTime.Now;
                var wxNewsInfo = new WxNewsShow();
                wxNewsInfo.MediaId = newRes.media_id;
                wxNewsInfo.FirstNewsTitle = lstNews[0].Title;
                wxNewsInfo.IsMultiple = lstNews.Count > 0 ? 1 : 0;
                wxNewsInfo.Creater = userName;
                wxNewsInfo.Updater = userName;
                wxNewsInfo.CreateTime = dtNow;
                wxNewsInfo.UpdateTime = dtNow;
                await repo.SaveAsync(wxNewsInfo);

                return new Result<string> { IsSucc = true, Message = "保存图文成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SaveWxNews:保存微信图文素材失败:" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<string> { Message = "保存微信图文素材失败！" };
        }

        /// <summary>
        /// 删除永久图文
        /// </summary>
        /// <returns></returns>
        [HttpPost("DelWxNews")]
        public async Task<Result<string>> DelWxNews([FromBody]WxNewsShow item)
        {
            try
            {
                var delRes = PubMediaApi.DeleteMaterial(item.MediaId);
                logHelper.Debug("DelWxNews:删除永久图文结果：" + delRes.JsonSerialize());
                if (!delRes.IsSuss)
                {
                    var msg = "删除永久图文失败：代码：" + delRes.errcode + "       " + delRes.errmsg;
                    logHelper.Error("DelWxNews:" + msg);
                    return new Result<string> { Message = msg };
                }

                item.IsDel = 1;
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                var upRes = await repo.UpdateAsync(item, new List<string> { "IsDel", "Updater", "UpdateTime" });
                return new Result<string> { IsSucc = upRes, Message = $"获取微信永久图文列表{(upRes ? "成功！" : "失败！")}" };
            }
            catch (Exception ex)
            {
                logHelper.Error("DelWxNews：获取微信永久图文列表失败：" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<string> { Message = "获取微信永久图文列表失败！" };
        }

        /// <summary>
        /// 同步永久图文列表
        /// </summary>
        /// <param name="objPara"></param>
        /// <returns></returns>
        [HttpPost("SyncNewsList")]
        public async Task<Result<string>> SyncNewsList([FromBody]JObject objPara)
        {
            try
            {
                int count = Convert.ToInt32(objPara["count"]);
                // 所有素材
                var lstAll = new List<PubNews>();
                var lstNewsIds = new List<string>();
                var startInd = 0;
                int pageSize = 20;
                var syncRes = PubMediaApi.GetMaterialNewsList(startInd, pageSize);
                logHelper.Debug("SyncNewsList:同步微信公众号永久图文素材结果:" + syncRes.JsonSerialize());
                if (!syncRes.IsSuss)
                {
                    var msg = "同步微信公众号永久图文素材失败：" + syncRes.errcode + "     " + syncRes.errmsg;
                    logHelper.Error("SyncNewsList:" + msg);
                    return new Result<string> { Message = msg };
                }

                // 总数
                var allCount = syncRes.total_count;
                if (allCount > count)
                {
                    allCount = count;
                }

                lstAll.AddRange(syncRes.item);
                await SyncWxNews(lstNewsIds, syncRes);

                // 循环同步
                while (syncRes.IsSuss && lstAll.Count < allCount)
                {
                    startInd += pageSize;
                    syncRes = PubMediaApi.GetMaterialNewsList(startInd, pageSize);
                    logHelper.Debug("SyncNewsList:同步微信公众号永久图文素材结果:" + syncRes.JsonSerialize());
                    if (syncRes.IsSuss)
                    {
                        lstAll.AddRange(syncRes.item);
                        await SyncWxNews(lstNewsIds, syncRes);
                    }
                }

                return new Result<string> { IsSucc = true, Message = "获取微信永久图文列表成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SyncNewsList：获取微信永久图文列表失败：" + ex.Message + "        " + ex.StackTrace);
            }

            return new Result<string> { Message = "获取微信永久图文列表失败！" };
        }

        /// <summary>
        /// 保存微信永久图文消息
        /// </summary>
        /// <param name="lstNewsIds"></param>
        /// <param name="syncRes"></param>
        private async Task SyncWxNews(List<string> lstNewsIds, PubNewsResult syncRes)
        {
            // 保存永久图文消息
            var dtNow = DateTime.Now;
            var userName = currentUser.UserName;
            foreach (var item in syncRes.item)
            {
                try
                {
                    // 保存图文消息
                    var exist = await repo.FirstOrDefaultAsync<WxNews>(x => x.MediaId == item.media_id);
                    if (exist == null)
                    {
                        exist = new WxNews();
                        exist.Creater = userName;
                        exist.CreateTime = dtNow;
                        exist.MediaId = item.media_id;
                    }

                    var lstDets = item.content.news_item;
                    exist.FirstNewsTitle = lstDets[0].title;
                    exist.IsMultiple = lstDets.Count > 1 ? 1 : 0;
                    exist.Updater = userName;
                    exist.UpdateTime = dtNow;
                    exist.IsDel = 0;
                    await repo.SaveAsync(exist);
                    lstNewsIds.Add(exist.MediaId);

                    var lstDetIds = new List<long>();
                    var lstNewsTemp = new List<WxNewsDetail> { };
                    foreach (var det in lstDets)
                    {
                        // 保存图文详情
                        var detTemp = new WxNewsDetail();
                        detTemp.MediaId = exist.MediaId;
                        detTemp.Title = det.title;
                        detTemp.ThumbMediaId = det.thumb_media_id;
                        detTemp.ThumbUrl = det.thumb_url;
                        detTemp.Author = det.author;
                        detTemp.Digest = det.digest;
                        detTemp.ShowCoverPic = det.show_cover_pic;
                        detTemp.Content = det.content;
                        detTemp.ContentSourceUrl = det.content_source_url;
                        detTemp.WxUrl = det.url;
                        detTemp.IsDel = 0;
                        detTemp.Creater = userName;
                        detTemp.Updater = userName;
                        detTemp.CreateTime = dtNow;
                        detTemp.UpdateTime = dtNow;
                        //await repo.SaveAsync(detTemp);
                        //lstDetIds.Add(detTemp.Id);
                        lstNewsTemp.Add(detTemp);
                    }

                    await repo.SaveAsync(lstNewsTemp);
                    lstDetIds = lstNewsTemp.Select(x => x.Id).ToList();

                    // 删除以前旧的图文详情信息
                    if (lstDetIds.Count > 0)
                    {
                        var sqlStr = string.Format(" UPDATE WxNewsDetail SET IsDel = 1, Updater = '{0}', UpdateTime = '{1}' WHERE MediaId = {2} AND Id NOT IN ({3}) ",
                            userName, dtNow, exist.MediaId, lstDetIds);
                        await repo.ExecuteCommandAsync(sqlStr);
                    }
                }
                catch (Exception ex)
                {
                    logHelper.Error("SyncWxNews:保存永久图文失败:" + ex.Message + "     " + ex.StackTrace);
                }
            }
        }

        #endregion
    }
}