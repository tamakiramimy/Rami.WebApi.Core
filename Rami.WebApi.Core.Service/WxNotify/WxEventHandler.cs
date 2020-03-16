using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.Wechat.Core.Public;
using Microsoft.AspNetCore.Http;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 微信推送Event处理
    /// </summary>
    public class WxEventHandler
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxEventHandler> logHelper;

        /// <summary>
        /// 微信自动通用应答
        /// </summary>
        private readonly WxAutoComResponse wxAutoComResponse;

        /// <summary>
        /// 微信推送扫码事件处理
        /// </summary>
        private readonly WxEventScanHelper wxEventScanHelper;

        /// <summary>
        /// 微信推送菜单点击事件处理
        /// </summary>
        private readonly WxEventClickHelper wxEventClickHelper;

        /// <summary>
        /// 微信数据库日志帮助类
        /// </summary>
        private readonly WxDbLogHelper wxDbLogHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="logHelper"></param>
        /// <param name="wxAutoComResponse"></param>
        /// <param name="wxEventScanHelper"></param>
        /// <param name="wxEventClickHelper"></param>
        /// <param name="wxDbLogHelper"></param>
        public WxEventHandler(IRepository repo, ILogHelper<WxEventHandler> logHelper, WxAutoComResponse wxAutoComResponse,
            WxEventScanHelper wxEventScanHelper, WxEventClickHelper wxEventClickHelper, WxDbLogHelper wxDbLogHelper)
        {
            this.repo = repo;
            this.logHelper = logHelper;
            this.wxAutoComResponse = wxAutoComResponse;
            this.wxEventScanHelper = wxEventScanHelper;
            this.wxEventClickHelper = wxEventClickHelper;
            this.wxDbLogHelper = wxDbLogHelper;
        }

        /// <summary>
        /// 处理企业号推送事件
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        public async Task<string> DealQyEvent(PubReceiveMsgCData recMsg)
        {
            try
            {
                switch (recMsg.Event)
                {
                    case PubEventType.subscribe:
                        await DealWxSubscribe(recMsg);
                        // 关注后判断扫码事件处理
                        await DealWxscan(recMsg);
                        break;
                    case PubEventType.unsubscribe:
                        await DealWxUnsubscribe(recMsg);
                        break;
                    case PubEventType.scan:
                        await DealWxscan(recMsg);
                        break;
                    case PubEventType.location:
                        logHelper.Debug("location");
                        break;
                    case PubEventType.location_select:
                        logHelper.Debug("location");
                        break;
                    case PubEventType.click:
                        await DealWxClick(recMsg);
                        break;
                    case PubEventType.view:
                        logHelper.Debug("view");
                        break;
                    case PubEventType.poi_check_notify:
                        logHelper.Debug("poi_check_notify");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                logHelper.Error("DealQyEvent:处理企业号推送事件失败：" + ex.Message + "        " + ex.StackTrace);
            }

            return wxAutoComResponse.ResponseOK(); ;
        }

        /// <summary>
        ///  关注
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        private async Task<string> DealWxSubscribe(PubReceiveMsgCData recMsg)
        {
            // 关注记录
            await wxDbLogHelper.SaveSubLog(recMsg, 0);

            // 发送关注消息
            var timeStamp = ComHelper.ConvertDateTimeInt(DateTime.Now);
            var wxUser = await repo.FirstOrDefaultAsync<WxUserInfo>(x => x.OpenId == recMsg.FromUserName);
            var subConf = await repo.FirstOrDefaultAsync<DbConfig>(x => x.ConfigName == "subscribeText");
            if (wxUser != null && subConf != null)
            {
                var msg = subConf.ConfigValue.Replace("{nickname}", wxUser.NickName);
                return await wxAutoComResponse.SendWxText(recMsg.FromUserName, recMsg.ToUserName, timeStamp, msg);
            }

            return wxAutoComResponse.ResponseOK(); ;
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="recMsg"></param>
        private async Task<string> DealWxUnsubscribe(PubReceiveMsgCData recMsg)
        {
            // 取关记录
            await wxDbLogHelper.SaveSubLog(recMsg, 1);
            return wxAutoComResponse.ResponseOK(); ;
        }

        /// <summary>
        /// 扫码操作
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        private async Task<string> DealWxscan(PubReceiveMsgCData recMsg)
        {
            // 扫码记录
            await wxDbLogHelper.SaveQrLog(recMsg);
            return await wxEventScanHelper.Deal(recMsg);
        }

        /// <summary>
        /// 菜单点击事件
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        private async Task<string> DealWxClick(PubReceiveMsgCData recMsg)
        {
            return await wxEventClickHelper.Deal(recMsg);
        }
    }
}
