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
    /// 微信自动通用应答
    /// </summary>
    public class WxAutoComResponse
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxAutoKeywordHelper> logHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        public WxAutoComResponse(ILogHelper<WxAutoKeywordHelper> logHelper)
        {
            this.logHelper = logHelper;
        }

        /// <summary>
        /// 发送微信应答
        /// </summary>
        /// <param name="gzhClient"></param>
        /// <param name="gzhSever"></param>
        /// <param name="timeStamp"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<string> SendWxText(string gzhClient, string gzhSever, int timeStamp, string text)
        {
            var msg = PubMsgApi.BuildTextMsg(gzhClient, gzhSever, timeStamp, text);
            return await AutoMsgResponse(msg);
        }

        /// <summary>
        /// 自动应答响应
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<string> AutoMsgResponse(string msg)
        {
            logHelper.Debug("AutoMsgResponse:Msg:" + msg);
            return await Task.FromResult(msg);
        }

        /// <summary>
        /// 返回Success
        /// </summary>
        /// <returns></returns>
        public string ResponseOK()
        {
            return "Success";
        }
    }
}
