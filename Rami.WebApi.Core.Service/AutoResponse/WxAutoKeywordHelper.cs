using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.Wechat.Core.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 关键字自动回复帮助类（客服消息）
    /// </summary>
    public class WxAutoKeywordHelper
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxAutoKeywordHelper> logHelper;

        /// <summary>
        /// 微信消息转换帮助类
        /// </summary>
        private readonly WxAutoConvertHelper wxAutoConvertHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="wxAutoConvertHelper"></param>
        public WxAutoKeywordHelper(ILogHelper<WxAutoKeywordHelper> logHelper, WxAutoConvertHelper wxAutoConvertHelper)
        {
            this.logHelper = logHelper;
            this.wxAutoConvertHelper = wxAutoConvertHelper;
        }

        /// <summary>
        /// 自动应答(客服消息)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<Result<string>> AutoRespond(WxAutoKeywordShow item, string openId)
        {
            try
            {
                // 自动推送类型
                Enum_ApMsgType type = ComHelper.GetEnumValueByStr<Enum_ApMsgType>(item.ContentType);
                // 发送客服消息预览
                PubApiResult apiRes = new PubApiResult();
                switch (type)
                {
                    case Enum_ApMsgType.Text:
                        item.TextContent = item.TextContent.Replace("{nickname}", string.Empty);
                        apiRes = PubKfApi.SendTextMsg(openId, item.TextContent);
                        break;
                    case Enum_ApMsgType.Image:
                        apiRes = PubKfApi.SendImageMsg(openId, item.MediaId);
                        break;
                    case Enum_ApMsgType.Voice:
                        apiRes = PubKfApi.SendVoiceMsg(openId, item.MediaId);
                        break;
                    case Enum_ApMsgType.News:
                        apiRes = PubKfApi.SendMpNewsMsg(openId, item.MediaId);
                        break;
                    case Enum_ApMsgType.Video:
                        apiRes = PubKfApi.SendVideoMsg(openId, item.MediaId, item.VideoThumbMediaId, item.VideoTitle, item.VideoDescription);
                        break;
                    case Enum_ApMsgType.BackNews:
                        var lstArts = await wxAutoConvertHelper.GetAutoKeywordBackNews(item);
                        apiRes = PubKfApi.SendNewsMsg(openId, lstArts);
                        break;
                    case Enum_ApMsgType.RedBag:
                        return await wxAutoConvertHelper.SendAutoKeywordRedPack(item, openId);
                    default:
                        break;
                }

                logHelper.Debug("PrevKeyword:预览返回结果：" + ComHelper.JsonSerialize(apiRes));
                if (!apiRes.IsSuss)
                {
                    var msg = "预览出错：" + apiRes.errcode + "    " + apiRes.errmsg;
                    logHelper.Error("PrevKeyword:" + msg);
                    return new Result<string> { Message = msg };
                }

                return new Result<string> { IsSucc = true, Message = "发送预览消息成功！" };
            }
            catch (Exception ex)
            {
                logHelper.Error("PrevKeyword:发送预览消息失败:" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "发送预览消息失败！" };
        }
    }
}
