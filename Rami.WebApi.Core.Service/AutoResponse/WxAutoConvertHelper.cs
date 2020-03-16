using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.Wechat.Core.Merchant.Public;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 微信消息转换帮助类
    /// </summary>
    public class WxAutoConvertHelper
    {
        /// <summary>
        /// 网站根目录
        /// </summary>
        private readonly string WebRoot = ComHelper.ContentRoot;

        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxAutoKeywordHelper> logHelper;

        /// <summary>
        /// 微信自动通用应答
        /// </summary>
        private readonly WxAutoComResponse wxAutoComResponse;

        /// <summary>
        /// 构造方法 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="logHelper"></param>
        /// <param name="wxAutoComResponse"></param>
        public WxAutoConvertHelper(IRepository repo, ILogHelper<WxAutoKeywordHelper> logHelper, WxAutoComResponse wxAutoComResponse)
        {
            this.repo = repo;
            this.logHelper = logHelper;
            this.wxAutoComResponse = wxAutoComResponse;
        }

        #region 公众号自动回复（xml消息）

        /// <summary>
        /// 把永久图文转成自动应答图文（公众号自动回复）
        /// </summary>
        /// <param name="autoKeyword"></param>
        /// <returns></returns>
        public async Task<List<PubNewsArticle>> GetResponseWxNews(WxAutoKeywordShow autoKeyword)
        {
            var lstArt = new List<PubNewsArticle>();
            var lstNewDets = await repo.QueryAsync<WxNewsDetailShow>(x => x.IsDel == 0 && x.MediaId == autoKeyword.MediaId);
            foreach (var det in lstNewDets)
            {
                var temp = new PubNewsArticle();
                temp.Title = det.Title;
                temp.Description = det.Digest;
                temp.PicUrl = det.ThumbUrl;
                temp.Url = det.WxUrl;
                lstArt.Add(temp);
            }

            return lstArt;
        }

        /// <summary>
        /// GzhMarket后台文章 转化成 微信图文（公众号自动回复）
        /// </summary>
        /// <param name="autoKeyword"></param>
        /// <param name="lstIds"></param>
        /// <returns></returns>
        public async Task<List<PubNewsArticle>> GetResponseBackNews(WxAutoKeywordShow autoKeyword, List<long> lstIds)
        {
            // 微信图文列表
            var lstWxArts = new List<PubNewsArticle>();
            // 选定的GzhMarket后台图文列表
            var lstArticles = await repo.QueryAsync<WxBackNewsShow>(x => x.IsDel == 0 && lstIds.Contains(x.Id));
            if (autoKeyword.SendType == 0)
            {
                // 多图文随机推送一篇
                var randArt = ComHelper.GetRandomVal(lstArticles);
                lstArticles = new List<WxBackNewsShow> { randArt };
            }

            // 后台图文转微信图文
            foreach (var art in lstArticles)
            {
                var news = new PubNewsArticle();
                news.Title = art.Name;
                news.Description = art.SecondName;
                // 图片路径
                if (!string.IsNullOrEmpty(art.ImgUrlVir))
                {
                    news.PicUrl = ComHelper.UpdImgAbsPath(WebRoot, art.ImgUrlVir);
                }

                // 网址
                if (!string.IsNullOrEmpty(art.ArticleUrl))
                {
                    news.Url = art.ArticleUrl;
                    if (art.ArticleUrl.StartsWith("~/"))
                    {
                        news.Url = ComHelper.UpdImgAbsPath(WebRoot, art.ArticleUrl);
                    }
                }

                lstWxArts.Add(news);
            }

            return lstWxArts;
        }

        /// <summary>
        /// 自动推送红包（公众号自动回复）
        /// </summary>
        /// <param name="gzhClient"></param>
        /// <param name="gzhSever"></param>
        /// <param name="autoKeyword"></param>
        /// <param name="dtNow"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public async Task<string> SendResponseRedPack(string gzhClient, string gzhSever, WxAutoKeywordShow autoKeyword, DateTime dtNow, int timeStamp)
        {
            // 红包只能8点之后领取
            if (dtNow.Hour < 8)
            {
                return await wxAutoComResponse.SendWxText(gzhSever, gzhClient, timeStamp, "亲：微信红包将于8点到24点之间才能领取");
            }
            // 红包参数配置 金额|个数|祝福语|红包名称|红包备注
            if (autoKeyword.RedAmount <= 0 || autoKeyword.RedCount <= 0)
            {
                logHelper.Error("SendMsgByPush:红包配置失败！autoID：" + autoKeyword.Id);
                return await wxAutoComResponse.SendWxText(gzhClient, gzhSever, timeStamp, "获取红包失败，请稍候重试！");
            }
            // 红包已发放个数
            var sendCount = repo.SqlQuerySingle<int>($" SELECT COUNT(1) FROM WxAutoKeywordDetail WHERE AutoId = {autoKeyword.Id} ");
            if (sendCount >= autoKeyword.RedCount)
            {
                logHelper.Error("SendMsgByPush:红包已经抢完,autoID：" + autoKeyword.Id);
                return await wxAutoComResponse.SendWxText(gzhClient, gzhSever, timeStamp, "客官，您来晚了，红包已被先到的小伙伴抢完鸟，下次记得早点来哦。");
            }

            // 判断用户是否已经发放过红包
            var exist = repo.FirstOrDefault<WxAutoKeywordDetail>(x => x.Id == autoKeyword.Id && x.Opend == gzhClient);
            if (exist != null)
            {
                logHelper.Error("SendMsgByPush:重复领取红包,autoID：" + autoKeyword.Id + "       OpenID:" + gzhClient);
                return await wxAutoComResponse.SendWxText(gzhClient, gzhSever, timeStamp, "你已经领取过此红包，请把机会留给更多的人！");
            }

            // 发放红包
            var payRes = RedpackApi.SendPack(gzhClient, autoKeyword.RedAmount, autoKeyword.RedAct, autoKeyword.RedWish, autoKeyword.RedRemark, "");
            if (payRes.IsSucc)
            {
                //红包发送成功
                WxAutoKeywordDetail redHis = await SaveAutoKeywordHis(autoKeyword.Id, gzhClient);
                logHelper.Debug("SendMsgByPush:发送红包成功:" + redHis.JsonSerialize());
                return await wxAutoComResponse.SendWxText(gzhClient, gzhSever, timeStamp, "红包发放成功，请注意查收~~");
            }
            else
            {
                logHelper.Error("SendMsgByPush:红包发送失败，错误码:" + payRes.err_code + "      错误描述" + payRes.err_code_des);
                return await wxAutoComResponse.SendWxText(gzhClient, gzhSever, timeStamp, "微信服务器暂忙，请稍候重试。");
            }
        }

        #endregion

        #region 关键字回复（客服消息、预览）

        /// <summary>
        /// 后台图文转换成客服图文消息(关键字回复)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<List<PubKfArticle>> GetAutoKeywordBackNews(WxAutoKeywordShow item)
        {
            var lstArts = new List<PubKfArticle>();
            if (!string.IsNullOrEmpty(item.ArtIds))
            {
                // 选定的图文ids
                item.LstSelArtIds = item.ArtIds.StrSplitList().Select(x => Convert.ToInt64(x)).ToList();
                if (item.LstSelArtIds != null && item.LstSelArtIds.Count > 0)
                {
                    // 最终发送的图文列表
                    var lstBackAftNews = new List<WxBackNewsShow>();
                    // 选定的图文列表
                    var lstBackNews = await repo.QueryAsync<WxBackNewsShow>(x => x.IsDel == 0 && item.LstSelArtIds.Contains(x.Id));
                    if (item.SendType == 0)
                    {
                        // 随机图文
                        var temp = ComHelper.GetRandomVal(lstBackNews);
                        lstBackAftNews.Add(temp);
                    }
                    else
                    {
                        // 多图文
                        lstBackAftNews = lstBackNews;
                    }
                    // 转换图文
                    foreach (var news in lstBackAftNews)
                    {
                        var temp = new PubKfArticle();
                        temp.title = news.Name;
                        temp.description = news.SecondName;
                        temp.url = ComHelper.UpdImgAbsPath(WebRoot, news.ArticleUrl);
                        temp.picurl = ComHelper.UpdImgAbsPath(WebRoot, news.ImgUrlVir);
                        lstArts.Add(temp);
                    }
                }
            }

            return lstArts;
        }

        /// <summary>
        /// 发送红包消息(关键字回复)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<Result<string>> SendAutoKeywordRedPack(WxAutoKeywordShow item, string openId)
        {
            // 判断红包个数
            var sendCount = repo.SqlQuerySingle<int>($" SELECT COUNT(1) FROM WxAutoKeywordDetail WHERE AutoId = {item.Id} ");
            if (sendCount >= item.RedCount)
            {
                return new Result<string> { Message = "很抱歉，你来晚了，红包已经发送完毕！" };
            }
            // 判断用户是否领取过
            var exist = await repo.FirstOrDefaultAsync<WxAutoKeywordDetail>(x => x.AutoId == item.Id && x.Opend == openId);
            if (exist != null)
            {
                return new Result<string> { Message = "你已经领取过了，请勿重复领取，把机会留给没有参与的用户！" };
            }
            // 发送红包
            var redRes = RedpackApi.SendPack(openId, item.RedAmount, item.RedAct, item.RedWish, item.RedWish, "");
            logHelper.Debug("SendPack:发送红包结果：" + ComHelper.JsonSerialize(redRes));
            if (redRes.IsSucc)
            {
                //红包发送成功
                var autoHis = await SaveAutoKeywordHis(item.Id, openId);
                logHelper.Debug("SendPack:发送红包成功:" + autoHis.JsonSerialize());
                return new Result<string> { IsSucc = true, Message = "发送红包成功！" };
            }
            else
            {
                var msg = "红包发送失败:" + redRes.err_code + "    " + redRes.err_code_des;
                logHelper.Error("SendPack:" + msg);
                return new Result<string> { Message = msg };
            }
        }

        /// <summary>
        /// 保存关键字回复记录(关键字回复)(红包使用)
        /// </summary>
        /// <param name="autoId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<WxAutoKeywordDetail> SaveAutoKeywordHis(long autoId, string openId)
        {
            var dtNow = DateTime.Now;
            var autoHis = new WxAutoKeywordDetail();
            autoHis.AutoId = autoId;
            autoHis.Opend = openId;
            autoHis.CreateTime = dtNow;
            autoHis.Creater = "AutoResp";
            autoHis.Updater = "AutoResp";
            autoHis.CreateTime = dtNow;
            autoHis.UpdateTime = dtNow;
            await repo.SaveAsync(autoHis);
            return autoHis;
        }

        #endregion

        #region 定时推送（客服消息、预览）

        /// <summary>
        /// 后台图文转换成客服图文消息(定时推送)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<List<PubKfArticle>> GetAutoPushBackNews(WxAutoPushShow item)
        {
            var lstArts = new List<PubKfArticle>();
            if (!string.IsNullOrEmpty(item.ArtIds))
            {
                // 选定的图文ids
                item.LstSelArtIds = item.ArtIds.StrSplitList().Select(x => Convert.ToInt64(x)).ToList();
                if (item.LstSelArtIds != null && item.LstSelArtIds.Count > 0)
                {
                    // 最终发送的图文列表
                    var lstBackAftNews = new List<WxBackNewsShow>();
                    // 选定的图文列表
                    var lstBackNews = await repo.QueryAsync<WxBackNewsShow>(x => x.IsDel == 0 && item.LstSelArtIds.Contains(x.Id));
                    if (item.IsMul == 0)
                    {
                        // 随机图文
                        var temp = ComHelper.GetRandomVal(lstBackNews);
                        lstBackAftNews.Add(temp);
                    }
                    else
                    {
                        // 多图文
                        lstBackAftNews = lstBackNews;
                    }
                    // 转换图文
                    foreach (var news in lstBackAftNews)
                    {
                        var temp = new PubKfArticle();
                        temp.title = news.Name;
                        temp.description = news.SecondName;
                        temp.url = ComHelper.UpdImgAbsPath(WebRoot, news.ArticleUrl);
                        temp.picurl = ComHelper.UpdImgAbsPath(WebRoot, news.ImgUrlVir);
                        lstArts.Add(temp);
                    }
                }
            }

            return lstArts;
        }

        /// <summary>
        /// 发送红包消息(定时推送)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<Result<string>> SendAutoPushRedPack(WxAutoPushShow item, string openId)
        {
            // 判断红包个数
            var sendCount = repo.SqlQuerySingle<int>(" SELECT COUNT(1) FROM WxAutoPushDetail WHERE AutoId = @Id ", new { Id = item.Id });
            if (sendCount >= item.RedCount)
            {
                return new Result<string> { Message = "很抱歉，你来晚了，红包已经发送完毕！" };
            }
            // 判断用户是否领取过
            var exist = await repo.FirstOrDefaultAsync<WxAutoPushDetail>(x => x.PushId == item.Id && x.Opend == openId);
            if (exist != null)
            {
                return new Result<string> { Message = "你已经领取过了，请勿重复领取，把机会留给没有参与的用户！" };
            }
            // 发送红包
            var redRes = RedpackApi.SendPack(openId, item.RedAmount, item.RedAct, item.RedWish, item.RedWish, "");
            logHelper.Debug("SendPack:发送红包结果：" + ComHelper.JsonSerialize(redRes));
            if (redRes.IsSucc)
            {
                //红包发送成功
                var autoHis = await SaveAutoPushHis(item.Id, openId);
                logHelper.Debug("SendPack:发送红包成功:" + autoHis.JsonSerialize());
                return new Result<string> { IsSucc = true, Message = "发送红包成功！" };
            }
            else
            {
                var msg = "红包发送失败:" + redRes.err_code + "    " + redRes.err_code_des;
                logHelper.Error("SendPack:" + msg);
                return new Result<string> { Message = msg };
            }
        }

        /// <summary>
        /// 保存定时推送发送记录（客服消息、预览）
        /// </summary>
        /// <param name="pushId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<WxAutoPushDetail> SaveAutoPushHis(long pushId, string openId)
        {
            var dtNow = DateTime.Now;
            var autoHis = new WxAutoPushDetail();
            autoHis.PushId = pushId;
            autoHis.Opend = openId;
            autoHis.PushTime = dtNow;
            autoHis.Creater = "AutoPush";
            autoHis.Updater = "AutoPush";
            autoHis.CreateTime = dtNow;
            autoHis.UpdateTime = dtNow;
            await repo.SaveAsync(autoHis);
            return autoHis;
        }

        #endregion
    }
}
