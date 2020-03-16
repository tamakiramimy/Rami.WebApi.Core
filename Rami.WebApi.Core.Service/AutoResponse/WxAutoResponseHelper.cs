using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 微信自动回复帮助类
    /// </summary>
    public class WxAutoResponseHelper
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxAutoResponseHelper> logHelper;

        /// <summary>
        /// 微信自动通用应答
        /// </summary>
        private readonly WxAutoComResponse wxAutoComResponse;

        /// <summary>
        /// 微信消息转换帮助类
        /// </summary>
        private readonly WxAutoConvertHelper wxAutoConvertHelper;

        /// <summary>
        /// 关键字自动回复帮助类（客服消息）
        /// </summary>
        private readonly WxAutoKeywordHelper wxAutoKeywordHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="repo"></param>
        /// <param name="wxAutoComResponse"></param>
        /// <param name="wxAutoConvertHelper"></param>
        /// <param name="wxAutoKeywordHelper"></param>
        public WxAutoResponseHelper(ILogHelper<WxAutoResponseHelper> logHelper, IRepository repo, WxAutoComResponse wxAutoComResponse,
            WxAutoConvertHelper wxAutoConvertHelper, WxAutoKeywordHelper wxAutoKeywordHelper)
        {
            this.logHelper = logHelper;
            this.repo = repo;
            this.wxAutoComResponse = wxAutoComResponse;
            this.wxAutoConvertHelper = wxAutoConvertHelper;
            this.wxAutoKeywordHelper = wxAutoKeywordHelper;
        }

        /// <summary>
        /// 处理关键字
        /// </summary>
        /// <param name="gzhClient"></param>
        /// <param name="gzhServer"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<string> DealWithKeyWord(string gzhClient, string gzhServer, string keyword)
        {
            // 当前时间
            var dtNow = DateTime.Now;
            var timeStamp = ComHelper.ConvertDateTimeInt(dtNow);
            // 当前微信用户信息
            var wxUser = await repo.FirstOrDefaultAsync<WxUserInfo>(x => x.OpenId == gzhClient);

            // 自动应答的内容
            WxAutoKeywordShow autoKeyword = null;
            // 超过1条的相同关键字的自动回复 改用客服消息发送
            var lstOthers = new List<WxAutoKeywordShow> { };
            if (keyword == "openid")
            {
                return await wxAutoComResponse.SendWxText(gzhClient, gzhServer, timeStamp, "您的OpenId为：" + gzhClient);
            }
            else
            {
                var sql = string.Format(@" SELECT * FROM WxAutoKeyword WHERE IsDel = 0 AND CONCAT('、',KeyWords,'、') LIKE '%、{0}、%' ORDER BY CreateTime DESC LIMIT 5 ", keyword);
                var lstKeywords = await repo.SqlQueryAsync<WxAutoKeywordShow>(sql);
                if (lstKeywords.Count > 0)
                {
                    autoKeyword = lstKeywords[0];
                }

                // 发送客服消息
                if (lstKeywords.Count > 1)
                {
                    lstOthers = lstKeywords.Skip(1).Take(4).ToList();
                }
            }

            // 公众号自动应答回复第一条关键字消息
            if (autoKeyword == null)
            {
                var msg = "DealQyText:无法找到对应关键字活动：" + keyword;
                logHelper.Debug(msg);
                return wxAutoComResponse.ResponseOK();
            }

            // 其他发送客服消息(不等待)
            SendKfAutoKeyword(wxUser, lstOthers);

            // 根据不同类型推送消息
            return await SendMsgByPush(gzhClient, gzhServer, autoKeyword, wxUser);
        }

        /// <summary>
        /// 超过1条的相同关键字的自动回复 改用客服消息发送
        /// </summary>
        /// <param name="wxUser"></param>
        /// <param name="lstKeywords"></param>
        public async Task SendKfAutoKeyword(WxUserInfo wxUser, List<WxAutoKeywordShow> lstKeywords)
        {
            foreach (var item in lstKeywords)
            {
                var res = await wxAutoKeywordHelper.AutoRespond(item, wxUser.OpenId);
                logHelper.Debug("SendKfAutoKeyword:自动应答发送结果：" + ComHelper.JsonSerialize(res));
            }
        }

        /// <summary>
        /// 根据不同类型推送消息
        /// </summary>
        /// <param name="gzhClient"></param>
        /// <param name="gzhSever"></param>
        /// <param name="autoKeyword"></param>
        /// <param name="wxUser"></param>
        /// <returns></returns>
        public async Task<string> SendMsgByPush(string gzhClient, string gzhSever, WxAutoKeywordShow autoKeyword, WxUserInfo wxUser)
        {
            logHelper.Debug("SendMsgByPush:autoPushing:" + autoKeyword.JsonSerialize());
            var dtNow = DateTime.Now;
            var timeStamp = ComHelper.ConvertDateTimeInt(dtNow);
            var apType = ComHelper.GetEnumValueByStr<Enum_ApMsgType>(autoKeyword.ContentType);
            switch (apType)
            {
                case Enum_ApMsgType.Text:
                    {
                        var msg = autoKeyword.TextContent.Replace("{nickname}", wxUser.NickName);
                        logHelper.Debug("SendMsgByPush:Msg:" + msg);
                        return await wxAutoComResponse.SendWxText(gzhClient, gzhSever, timeStamp, msg);
                    }
                case Enum_ApMsgType.Image:
                    {
                        var msg = PubMsgApi.BuildImageMsg(gzhClient, gzhSever, autoKeyword.MediaId, timeStamp);
                        return await wxAutoComResponse.AutoMsgResponse(msg);
                    }
                case Enum_ApMsgType.Voice:
                    {
                        var msg = PubMsgApi.BuildVoiceMsg(gzhClient, gzhSever, autoKeyword.MediaId, timeStamp);
                        return await wxAutoComResponse.AutoMsgResponse(msg);
                    }
                case Enum_ApMsgType.News:
                    {
                        var lstArt = await wxAutoConvertHelper.GetResponseWxNews(autoKeyword);
                        var msg = PubMsgApi.BuildArticleMsg(gzhClient, gzhSever, timeStamp, lstArt);
                        return await wxAutoComResponse.AutoMsgResponse(msg);
                    }
                case Enum_ApMsgType.Video:
                    {
                        var msg = PubMsgApi.BuildVideoMsg(gzhClient, gzhSever, autoKeyword.MediaId, autoKeyword.VideoTitle, autoKeyword.VideoDescription, timeStamp);
                        return await wxAutoComResponse.AutoMsgResponse(msg);
                    }
                case Enum_ApMsgType.BackNews:
                    {
                        var lstIds = autoKeyword.ArtIds.StrSplitList().Select(x => Convert.ToInt64(x)).ToList();
                        if (lstIds.Count > 0)
                        {
                            var lstWxArts = await wxAutoConvertHelper.GetResponseBackNews(autoKeyword, lstIds);
                            var msg = PubMsgApi.BuildArticleMsg(gzhClient, gzhSever, timeStamp, lstWxArts);
                            return await wxAutoComResponse.AutoMsgResponse(msg);
                        }
                    }
                    break;
                case Enum_ApMsgType.RedBag:
                    {
                        return await wxAutoConvertHelper.SendResponseRedPack(gzhClient, gzhSever, autoKeyword, dtNow, timeStamp);
                    }
                default:
                    break;
            }

            return wxAutoComResponse.ResponseOK();
        }
    }
}
