using Rami.WebApi.Core.Framework;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 微信推送菜单点击事件处理
    /// </summary>
    public class WxEventClickHelper
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxEventClickHelper> logHelper;

        /// <summary>
        /// 微信自动回复帮助类
        /// </summary>
        private readonly WxAutoResponseHelper wxAutoResponseHelper;

        /// <summary>
        /// 微信自动通用应答
        /// </summary>
        private readonly WxAutoComResponse wxAutoComResponse;

        /// <summary>
        /// 微信客服转接消息帮助类
        /// </summary>
        private readonly WxKfTransferHelper wxKfTransferHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="repo"></param>
        /// <param name="wxAutoResponseHelper"></param>
        /// <param name="wxAutoComResponse"></param>
        /// <param name="wxKfTransferHelper"></param>
        public WxEventClickHelper(ILogHelper<WxEventClickHelper> logHelper, IRepository repo, WxAutoResponseHelper wxAutoResponseHelper,
            WxAutoComResponse wxAutoComResponse, WxKfTransferHelper wxKfTransferHelper)
        {
            this.logHelper = logHelper;
            this.repo = repo;
            this.wxAutoResponseHelper = wxAutoResponseHelper;
            this.wxAutoComResponse = wxAutoComResponse;
            this.wxKfTransferHelper = wxKfTransferHelper;
        }

        /// <summary>
        /// 微信推送菜单点击事件处理
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        public async Task<string> Deal(PubReceiveMsgCData recMsg)
        {
            if (!string.IsNullOrEmpty(recMsg.EventKey))
            {
                var keyWord = recMsg.EventKey.Trim().ToLower();
                logHelper.Debug("WxEventClick:EventKey:" + keyWord);
                switch (keyWord)
                {
                    // 联系客服
                    case "gotoservices":
                        return await wxKfTransferHelper.ContactKf(recMsg);
                    // surprise 或者 其他文本
                    default:
                        return await wxAutoResponseHelper.DealWithKeyWord(recMsg.FromUserName, recMsg.ToUserName, keyWord);
                }
            }

            return wxAutoComResponse.ResponseOK(); ;
        }
    }
}
