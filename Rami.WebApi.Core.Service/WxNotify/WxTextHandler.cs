using Rami.WebApi.Core.Framework;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 公众号推送文本处理
    /// </summary>
    public class WxTextHandler
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxTextHandler> logHelper;

        /// <summary>
        /// 微信自动回复帮助类
        /// </summary>
        private readonly WxAutoResponseHelper wxAutoResponseHelper;

        /// <summary>
        /// 微信自动通用应答
        /// </summary>
        private readonly WxAutoComResponse wxAutoComResponse;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="repo"></param>
        /// <param name="wxAutoResponseHelper"></param>
        /// <param name="wxAutoComResponse"></param>
        public WxTextHandler(ILogHelper<WxTextHandler> logHelper, IRepository repo, WxAutoResponseHelper wxAutoResponseHelper,
            WxAutoComResponse wxAutoComResponse)
        {
            this.logHelper = logHelper;
            this.repo = repo;
            this.wxAutoResponseHelper = wxAutoResponseHelper;
            this.wxAutoComResponse = wxAutoComResponse;
        }

        /// <summary>
        /// 公众号推送Text处理
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        public async Task<string> DealQyText(PubReceiveMsgCData recMsg)
        {
            try
            {
                // 用户
                var gzhClient = recMsg.FromUserName;
                // 公众号
                var gzhServer = recMsg.ToUserName;
                // 文本内容
                var keyword = recMsg.Content.ToLower();
                return await wxAutoResponseHelper.DealWithKeyWord(gzhClient, gzhServer, keyword);
            }
            catch (Exception ex)
            {
                logHelper.Error("DealQyText:处理Text出错：" + ex.Message + "     " + ex.StackTrace);
            }

            return wxAutoComResponse.ResponseOK();
        }
    }
}
