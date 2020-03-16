using Rami.WebApi.Core.Framework;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 微信推送扫码事件处理
    /// </summary>
    public class WxEventScanHelper
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxEventScanHelper> logHelper;

        /// <summary>
        /// 微信自动通用应答
        /// </summary>
        private readonly WxAutoComResponse wxAutoComResponse;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="repo"></param>
        /// <param name="wxAutoComResponse"></param>
        public WxEventScanHelper(ILogHelper<WxEventScanHelper> logHelper, IRepository repo, WxAutoComResponse wxAutoComResponse)
        {
            this.logHelper = logHelper;
            this.repo = repo;
            this.wxAutoComResponse = wxAutoComResponse;
        }

        /// <summary>
        /// 微信推送扫码事件处理
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        public async Task<string> Deal(PubReceiveMsgCData recMsg)
        {
            await Task.CompletedTask;
            return wxAutoComResponse.ResponseOK(); ;
        }
    }
}
