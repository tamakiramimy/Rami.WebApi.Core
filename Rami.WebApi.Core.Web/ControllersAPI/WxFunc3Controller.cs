using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Rami.WebApi.Core.Web.Code;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 公众号API3
    /// </summary>
    [ApiAuthorize(AuthenticationSchemes = AuthHelper.JwtAuthScheme, Policy = AuthConst.ApiAuthSimple)]
    public class WxFunc3Controller : BaseController
    {
        /// <summary>
        /// 数据服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxFunc3Controller> logHelper;

        /// <summary>
        /// 当前用户
        /// </summary>
        private readonly CurrentUser currentUser;

        /// <summary>
        /// 关键字自动回复帮助类
        /// </summary>
        private readonly WxAutoKeywordHelper wxAutoKeywordHelper;

        /// <summary>
        /// 定时推送（客服消息）
        /// </summary>
        private readonly WxAutoPushHelper wxAutoPushHelper;

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
        /// <param name="wxAutoKeywordHelper"></param>
        /// <param name="cacheHelper"></param>
        /// <param name="wxAutoPushHelper"></param>
        public WxFunc3Controller(IRepository repo, ILogHelper<WxFunc3Controller> logHelper, CurrentUser currentUser,
            WxAutoKeywordHelper wxAutoKeywordHelper, ICacheHelper cacheHelper, WxAutoPushHelper wxAutoPushHelper)
        {
            this.repo = repo;
            this.logHelper = logHelper;
            this.currentUser = currentUser;
            this.wxAutoKeywordHelper = wxAutoKeywordHelper;
            this.cacheHelper = cacheHelper;
            this.wxAutoPushHelper = wxAutoPushHelper;
        }

        #region 关键字自动应答

        /// <summary>
        /// 获取自动回复关键字配置
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetAutoKeywords")]
        public async Task<Result<Pager<WxAutoKeywordShow>>> GetAutoKeywords([FromBody]Para<WxAutoKeywordShow> para)
        {
            try
            {
                var query = para.Entity;
                if (query != null)
                {
                    if (!string.IsNullOrEmpty(query.AutoName))
                    {
                        para.Filter = para.Filter.And(x => x.AutoName.Contains(query.AutoName));
                    }

                    if (!string.IsNullOrEmpty(query.KeyWords))
                    {
                        para.Filter = para.Filter.And(x => x.KeyWords.Contains(query.KeyWords));
                    }
                }

                para.OrderKey = " IsDel Asc,CreateTime Desc ";
                var pageRes = await repo.QueryPageAsync(para);
                foreach (var item in pageRes.Datas)
                {
                    item.LstSelArtIds = new List<long>();
                    if (item.ContentType == Enum_ApMsgType.BackNews.ToString() && !string.IsNullOrEmpty(item.ArtIds))
                    {
                        item.LstSelArtIds = item.ArtIds.StrSplitList().Select(x => Convert.ToInt64(x)).ToList();
                    }
                }

                return new Result<Pager<WxAutoKeywordShow>> { IsSucc = true, Data = pageRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetAutoKeywords:获取自动回复关键字配置失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<Pager<WxAutoKeywordShow>> { Message = "获取自动回复关键字配置失败！" };
        }

        /// <summary>
        /// 保存自动回复关键字配置
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveAutoKeyword")]
        public async Task<Result<string>> SaveAutoKeyword([FromBody]WxAutoKeywordShow item)
        {
            try
            {
                // 多图文
                if (item.LstSelArtIds != null && item.LstSelArtIds.Count > 0)
                {
                    item.ArtIds = string.Join(",", item.LstSelArtIds);
                }

                // 视频
                if (item.ContentType == Enum_ApMsgType.Video.ToString())
                {
                    var vedio = await repo.FirstOrDefaultAsync<WxMediaShow>(x => x.MediaId == item.MediaId);
                    if (vedio != null)
                    {
                        item.VideoTitle = vedio.Title;
                        item.VideoDescription = vedio.Introduction;
                    }
                }

                var dtNow = DateTime.Now;
                var user = currentUser.UserName;
                if (!item.CreateTime.HasValue)
                {
                    item.Creater = user;
                    item.CreateTime = dtNow;
                }

                item.Updater = user;
                item.UpdateTime = dtNow;
                await repo.SaveAsync(item);
                return new Result<string> { IsSucc = true, Message = "保存自动回复关键字配置成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SaveAutoKeywords:保存自动回复关键字配置失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "保存自动回复关键字配置失败！" };
        }

        /// <summary>
        /// 发送预览消息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("PrevKeyword")]
        public async Task<Result<string>> PrevKeyword([FromBody]WxAutoKeywordShow item)
        {
            if (!string.IsNullOrEmpty(item.OpenId))
            {
                return await wxAutoKeywordHelper.AutoRespond(item, item.OpenId);
            }
            else
            {
                if (string.IsNullOrEmpty(item.PrevNick))
                {
                    return new Result<string> { Message = "请输入要预览的微信用户昵称！" };
                }

                // 用户信息
                var wxUser = await repo.FirstOrDefaultAsync<WxUserInfo>(x => x.NickName == item.PrevNick && x.IsSubscribe == 1);
                if (wxUser == null)
                {
                    return new Result<string> { Message = "昵称对应的微信用户不存在！" };
                }

                return await wxAutoKeywordHelper.AutoRespond(item, wxUser.OpenId);
            }
        }

        /// <summary>
        /// 更新关键字状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("UpdAutoKeyword")]
        public async Task<Result<string>> UpdAutoKeyword([FromBody]WxAutoKeywordShow item)
        {
            try
            {
                item.IsDel = 1 - item.IsDel;
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                await repo.UpdateAsync(item, new List<string> { "IsDel", "Updater", "UpdateTime" });
                return new Result<string> { IsSucc = true, Message = "更新关键字状态成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("UpdAutoKe:更新关键字状态失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "更新关键字状态失败！" };
        }

        /// <summary>
        /// 根据Id获取关键字详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetAutoKeyDetById")]
        public async Task<Result<dynamic>> GetAutoKeyDetById([FromQuery]int id)
        {
            var auto = await repo.FirstOrDefaultAsync<WxAutoKeywordShow>(x => x.Id == id);
            if (auto != null)
            {
                var autoType = ComHelper.GetEnumValueByStr<Enum_ApMsgType>(auto.ContentType);
                dynamic lstOpts1 = null;
                dynamic lstOpts2 = null;
                switch (autoType)
                {
                    case Enum_ApMsgType.Image:
                        lstOpts1 = await GetMediaInfoByTypeId(PubMediaType.image, auto.MediaId);
                        break;
                    case Enum_ApMsgType.Voice:
                        lstOpts1 = await GetMediaInfoByTypeId(PubMediaType.voice, auto.MediaId);
                        break;
                    case Enum_ApMsgType.Video:
                        lstOpts1 = await GetMediaInfoByTypeId(PubMediaType.image, auto.VideoThumbMediaId);
                        lstOpts2 = await GetMediaInfoByTypeId(PubMediaType.video, auto.MediaId);
                        break;
                    case Enum_ApMsgType.News:
                        lstOpts1 = await GetWxNewsById(auto.MediaId, lstOpts1);
                        break;
                    case Enum_ApMsgType.BackNews:
                        lstOpts1 = await GetWxBackNewsById(auto.ArtIds, lstOpts1);
                        auto.LstSelArtIds = auto.ArtIds.StrSplitList().Select(x => Convert.ToInt64(x)).ToList();
                        break;
                    default:
                        break;
                }

                return new Result<dynamic> { IsSucc = true, Data = new { auto, lstOpts1, lstOpts2 } };
            }

            return new Result<dynamic> { Message = "获取自动回复详情失败！" };
        }

        /// <summary>
        /// 根据素材类型和id获取素材下拉列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<dynamic> GetMediaInfoByTypeId(PubMediaType type, string id)
        {
            // todo orm如果直接用linq x.type == type.ToString() 无法查询数据
            var mediaType = type.ToString();
            var media = await repo.FirstOrDefaultAsync<WxMediaShow>(x => x.Type == mediaType && x.MediaId == id);
            if (media != null)
            {
                var optTemp = new { id = media.MediaId, text = media.Title };
                return new List<dynamic> { optTemp };
            }

            return null;
        }

        /// <summary>
        /// 根据Id获取公众号图文信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lstOpts1"></param>
        /// <returns></returns>
        private async Task<dynamic> GetWxNewsById(string id, dynamic lstOpts1)
        {
            var news = await repo.FirstOrDefaultAsync<WxNewsShow>(x => x.MediaId == id);
            if (news != null)
            {
                var optTemp = new { id = news.MediaId, text = news.FirstNewsTitle };
                lstOpts1 = new List<dynamic> { optTemp };
            }

            return lstOpts1;
        }

        /// <summary>
        /// 根据Id获取后台图文信息
        /// </summary>
        /// <param name="newsIds"></param>
        /// <param name="lstOpts1"></param>
        /// <returns></returns>
        private async Task<dynamic> GetWxBackNewsById(string newsIds, dynamic lstOpts1)
        {
            var lstSelArtIds = newsIds.StrSplitList().Select(x => Convert.ToInt64(x)).ToList();
            if (lstSelArtIds.Count > 0)
            {
                var lstBackNews = await repo.QueryAsync<WxBackNewsShow>(x => lstSelArtIds.Contains(x.Id));
                lstOpts1 = lstBackNews.Select(x => new { x.Id, x.Name }).ToList();
            }

            return lstOpts1;
        }

        #endregion

        #region 自动推送配置

        /// <summary>
        /// 获取国家列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCountry")]
        public Result<List<string>> GetCountry()
        {
            try
            {
                var lstRes = cacheHelper.GetCountryByName(1, "");
                return new Result<List<string>> { IsSucc = true, Data = lstRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetCountry:根据上级获取下级省市信息失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<List<string>> { Message = "获取国家列表失败！" };
        }

        /// <summary>
        /// 根据上级获取下级省市信息失败
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        [HttpGet("GetChildArea")]
        public Result<List<string>> GetChildArea([FromQuery]int type, string parent)
        {
            try
            {
                var lstRes = cacheHelper.GetCountryByName(type, parent);
                return new Result<List<string>> { IsSucc = true, Data = lstRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetChildArea:根据上级获取下级省市信息失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<List<string>> { Message = "根据上级获取下级省市信息失败！" };
        }

        /// <summary>
        /// 获取自动推送配置
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("GetAutoPushList")]
        public async Task<Result<Pager<WxAutoPushShow>>> GetAutoPushList([FromBody]Para<WxAutoPushShow> para)
        {
            try
            {
                para.SQL = @" SELECT a.*,c.PushCount FROM WxAutoPush a
LEFT JOIN 
(SELECT b.PushId,COUNT(1) AS PushCount FROM WxAutoPushDetail b GROUP BY b.PushId) c
ON c.PushId = a.Id
WHERE a.Id > 0
 {0}
ORDER BY a.IsDel ASC,a.PushTime Desc  ";
                var strWhere = string.Empty;
                if (para.Entity != null)
                {
                    var item = para.Entity;
                    if (!string.IsNullOrEmpty(item.PushName))
                    {
                        strWhere += string.Format(" AND a.PushName LIKE '%{0}%' ", item.PushName);
                    }
                }

                para.SQL = string.Format(para.SQL, strWhere);
                var pageRes = await repo.QueryPageAsync(para);
                foreach (var item in pageRes.Datas)
                {
                    if (!string.IsNullOrEmpty(item.PushingSex))
                    {
                        item.LstSelSex = item.PushingSex.StrSplitList().ToList();
                    }

                    if (item.ContentType == Enum_ApMsgType.BackNews.ToString() && !string.IsNullOrEmpty(item.ArtIds))
                    {
                        item.LstSelArtIds = item.ArtIds.StrSplitList().Select(x => Convert.ToInt64(x)).ToList();
                    }
                }

                return new Result<Pager<WxAutoPushShow>> { IsSucc = true, Data = pageRes };
            }
            catch (Exception ex)
            {
                logHelper.Error("GetAutoPushList:获取自动推送配置失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<Pager<WxAutoPushShow>> { Message = "获取自动推送配置失败！" };
        }

        /// <summary>
        /// 保存自动推送配置
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("SaveAutoPush")]
        public async Task<Result<string>> SaveAutoPush([FromBody]WxAutoPushShow item)
        {
            try
            {
                // 多图文
                if (item.LstSelArtIds != null && item.LstSelArtIds.Count > 0)
                {
                    item.ArtIds = string.Join(",", item.LstSelArtIds);
                }

                // 性别
                if (item.LstSelSex != null && item.LstSelSex.Count > 0)
                {
                    item.PushingSex = string.Join(",", item.LstSelSex);
                }

                // 视频
                if (item.ContentType == Enum_ApMsgType.Video.ToString())
                {
                    var vedio = await repo.FirstOrDefaultAsync<WxMediaShow>(x => x.MediaId == item.MediaId);
                    if (vedio != null)
                    {
                        item.VideoTitle = vedio.Title;
                        item.VideoDescription = vedio.Introduction;
                    }
                }

                var dtNow = DateTime.Now;
                var user = currentUser.UserName;
                if (!item.CreateTime.HasValue)
                {
                    item.Creater = user;
                    item.CreateTime = dtNow;
                }

                item.Updater = user;
                item.UpdateTime = dtNow;
                await repo.SaveAsync(item);
                return new Result<string> { IsSucc = true, Message = "保存自动推送配置成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("SaveAutoPush:保存自动推送配置失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "保存自动推送配置失败！" };
        }

        /// <summary>
        /// 发送预览消息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("PrevAutoPush")]
        public async Task<Result<string>> PrevAutoPush([FromBody]WxAutoPushShow item)
        {
            if (!string.IsNullOrEmpty(item.OpenID))
            {
                return await wxAutoPushHelper.AutoRespond(item, item.OpenID);
            }
            else
            {
                if (string.IsNullOrEmpty(item.PrevNick))
                {
                    return new Result<string> { Message = "请输入要预览的微信用户昵称！" };
                }

                // 用户信息
                var wxUser = await repo.FirstOrDefaultAsync<WxUserInfo>(x => x.NickName == item.PrevNick && x.IsSubscribe == 1);
                if (wxUser == null)
                {
                    return new Result<string> { Message = "昵称对应的微信用户不存在！" };
                }

                return await wxAutoPushHelper.AutoRespond(item, wxUser.OpenId);
            }
        }

        /// <summary>
        /// 更新自动推送状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("UpdAutoPush")]
        public async Task<Result<string>> UpdAutoPush([FromBody]WxAutoPushShow item)
        {
            try
            {
                item.IsDel = 1 - item.IsDel;
                item.Updater = currentUser.UserName;
                item.UpdateTime = DateTime.Now;
                await repo.UpdateAsync(item, new List<string> { "IsDel", "Updater", "UpdateTime" });
                return new Result<string> { IsSucc = true, Message = "更新自动推送状态成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("UpdAutoPush:更新自动推送状态失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "更新自动推送状态失败！" };
        }

        /// <summary>
        /// 根据Id获取自动回复详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetAutoPushDetById")]
        public async Task<Result<dynamic>> GetAutoPushDetById([FromQuery]int id)
        {
            var auto = await repo.FirstOrDefaultAsync<WxAutoPushShow>(x => x.Id == id);
            if (auto != null)
            {
                // 多选选项初始化
                auto.LstSelSex = auto.PushingSex.StrSplitList().ToList();

                // 下拉选项初始化
                var autoType = ComHelper.GetEnumValueByStr<Enum_ApMsgType>(auto.ContentType);
                dynamic lstOpts1 = null;
                dynamic lstOpts2 = null;
                switch (autoType)
                {
                    case Enum_ApMsgType.Image:
                        lstOpts1 = await GetMediaInfoByTypeId(PubMediaType.image, auto.MediaId);
                        break;
                    case Enum_ApMsgType.Voice:
                        lstOpts1 = await GetMediaInfoByTypeId(PubMediaType.voice, auto.MediaId);
                        break;
                    case Enum_ApMsgType.Video:
                        lstOpts1 = await GetMediaInfoByTypeId(PubMediaType.image, auto.VideoThumbMediaId);
                        lstOpts2 = await GetMediaInfoByTypeId(PubMediaType.video, auto.MediaId);
                        break;
                    case Enum_ApMsgType.News:
                        lstOpts1 = await GetWxNewsById(auto.MediaId, lstOpts1);
                        break;
                    case Enum_ApMsgType.BackNews:
                        lstOpts1 = await GetWxBackNewsById(auto.ArtIds, lstOpts1);
                        break;
                    default:
                        break;
                }

                return new Result<dynamic> { IsSucc = true, Data = new { auto, lstOpts1, lstOpts2 } };
            }

            return new Result<dynamic> { Message = "获取自动回复详情失败！" };
        }

        #endregion

        #region 微信素材

        /// <summary>
        /// 获取微信素材(非图文)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("GetWxMediaByType")]
        public async Task<dynamic> GetWxMediaByType([FromQuery]string type, string name = "")
        {
            try
            {
                var strSql = string.Empty;
                if (string.IsNullOrEmpty(name))
                {
                    strSql = string.Format(" SELECT MediaId AS id,Title AS text FROM WxMedia WHERE IsDel = 0 AND Type = '{0}' ORDER BY UpdateTime DESC Limit 20 ", type);
                }
                else
                {
                    strSql = string.Format(" SELECT MediaId AS id,Title AS text FROM WxMedia WHERE IsDel = 0 AND Type = '{0}' AND Title LIKE '%{1}%' ORDER BY UpdateTime DESC Limit 20 ", type, name);
                }

                var lstRes = await repo.SqlQueryAsync<dynamic>(strSql);
                return lstRes;
            }
            catch (Exception ex)
            {
                logHelper.Error("GetWxMediaByType:获取微信素材失败：" + ex.Message + "       " + ex.StackTrace);
            }

            return new List<dynamic> { };
        }

        /// <summary>
        /// 获取微信图文素材
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("GetWxNewsByType")]
        public async Task<dynamic> GetWxNewsByType([FromQuery]string name = "")
        {
            try
            {
                var strSql = string.Empty;
                if (string.IsNullOrEmpty(name))
                {
                    strSql = " SELECT MediaId AS id,FirstNewsTitle AS text FROM WxNews WHERE IsDel = 0 ORDER BY UpdateTime DESC Limit 20 ";
                }
                else
                {
                    strSql = string.Format(" SELECT MediaId AS id,FirstNewsTitle AS text FROM WxNews WHERE IsDel = 0 AND FirstNewsTitle LIKE '%{0}%' ORDER BY UpdateTime DESC Limit 20 ", name);
                }

                var lstRes = await repo.SqlQueryAsync<dynamic>(strSql);
                return lstRes;
            }
            catch (Exception ex)
            {
                logHelper.Error("GetWxNewsByType:获取微信素材失败：" + ex.Message + "       " + ex.StackTrace);
            }

            return new List<dynamic> { };
        }

        /// <summary>
        /// 获取后台图文列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("GetWxBackNews")]
        public async Task<dynamic> GetWxBackNews([FromQuery]string name = "")
        {
            try
            {
                var strSql = string.Empty;
                if (string.IsNullOrEmpty(name))
                {
                    strSql = " SELECT Id,Name FROM wxbacknews WHERE IsDel = 0 ORDER BY UpdateTime Desc LIMIT 20 ";
                }
                else
                {
                    strSql = string.Format(" SELECT Id,Name FROM wxbacknews WHERE IsDel = 0 AND Name LIKE '%{0}%' ORDER BY UpdateTime DESC Limit 20 ", name);
                }

                var lstRes = await repo.SqlQueryAsync<dynamic>(strSql);
                return lstRes;
            }
            catch (Exception ex)
            {
                logHelper.Error("GetWxBackNews:获取后台图文列表：" + ex.Message + "       " + ex.StackTrace);
            }

            return new List<dynamic> { };
        }

        /// <summary>
        /// 获取图文HTML内容
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetNewsHtml")]
        public async Task<Result<string>> GetNewsHtml([FromQuery]string type, string id)
        {
            type = type.ToLower();
            var html = string.Empty;
            if (type == "backnews")
            {
                var backNews = await repo.FirstOrDefaultAsync<WxBackNewsShow>(x => x.Id == Convert.ToInt64(id));
                html = backNews?.HtmlContent;
            }
            else if (type == "wxnews")
            {
                var wxNewsDet = await repo.FirstOrDefaultAsync<WxNewsDetailShow>(x => x.Id == Convert.ToInt64(id));
                html = wxNewsDet?.Content;
            }

            return new Result<string> { IsSucc = true, Data = html };
        }

        #endregion
    }
}