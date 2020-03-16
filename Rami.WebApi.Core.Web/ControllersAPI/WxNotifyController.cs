using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Rami.Wechat.Core.Public;
using Rami.Wechat.Core.Public.Tencent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 微信通知API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WxNotifyController : ControllerBase
    {
        /// <summary>
        /// 验证服务
        /// </summary>
        private readonly WXBizMsgCrypt QyCrypt = new WXBizMsgCrypt(PubInterface.Conf.AppToken, PubInterface.Conf.EncodingAesKey, PubInterface.Conf.AppId);

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxNotifyController> logHelper;

        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 推送事件处理
        /// </summary>
        private readonly WxEventHandler eventHandler;

        /// <summary>
        /// 推送Text处理
        /// </summary>
        private readonly WxTextHandler textHandler;

        /// <summary>
        /// 微信自动通用应答
        /// </summary>
        private readonly WxAutoComResponse wxAutoComResponse;

        /// <summary>
        /// 微信数据库日志帮助类
        /// </summary>
        private readonly WxDbLogHelper wxDbLogHelper;

        /// <summary>
        /// 微信客服转接消息帮助类
        /// </summary>
        private readonly WxKfTransferHelper wxKfTransferHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="repo"></param>
        /// <param name="eventHandler"></param>
        /// <param name="textHandler"></param>
        /// <param name="wxAutoComResponse"></param>
        /// <param name="wxDbLogHelper"></param>
        /// <param name="wxKfTransferHelper"></param>
        public WxNotifyController(ILogHelper<WxNotifyController> logHelper, IRepository repo, WxEventHandler eventHandler,
            WxTextHandler textHandler, WxAutoComResponse wxAutoComResponse, WxDbLogHelper wxDbLogHelper, WxKfTransferHelper wxKfTransferHelper)
        {
            this.logHelper = logHelper;
            this.repo = repo;
            this.eventHandler = eventHandler;
            this.textHandler = textHandler;
            this.wxAutoComResponse = wxAutoComResponse;
            this.wxDbLogHelper = wxDbLogHelper;
            this.wxKfTransferHelper = wxKfTransferHelper;
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("HelloTest")]
        public string HelloTest()
        {
            return "Hello";
        }

        /// <summary>
        /// 验证服务器
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echostr"></param>
        /// <returns></returns>
        [HttpGet("Process")]
        public string Process([FromQuery]string signature, string timestamp, string nonce, string echostr)
        {
            logHelper.Debug("Process:" + Request.Host.Value);
            // 验签
            int ret = WXBizMsgCrypt.VerifySignature(PubInterface.Conf.AppToken, timestamp, nonce, "", signature);

            // 判断是否验签成功
            if (ret != 0)
            {
                var msg = "Process：微信验证服务器失败：错误代码：" + ret;
                logHelper.Debug(msg);
                return msg;
            }

            // 验证签名成功返回结果
            return echostr;
        }

        /// <summary>
        /// 微信消息推送
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="openid"></param>
        /// <param name="encrypt_type"></param>
        /// <param name="msg_signature"></param>
        /// <returns></returns>
        [HttpPost("Process")]
        public async Task<dynamic> Process([FromQuery]string signature, string timestamp, string nonce, string openid, string encrypt_type, string msg_signature)
        {
            logHelper.Debug("Process:请求Url:" + Request.Host.Value);
            // 响应
            var response = HttpContext.Response;

            // post过来的数据
            Stream stream = HttpContext.Request.Body;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            string postStr = System.Text.Encoding.UTF8.GetString(bytes);
            logHelper.Debug("Process:推送数据：" + postStr);

            // 验证数据签名
            string decryptMsg = string.Empty;
            int ret = -1;
            if (string.IsNullOrEmpty(PubInterface.Conf.EncodingAesKey))
            {
                // 明文验签
                ret = WXBizMsgCrypt.VerifySignature(PubInterface.Conf.AppToken, timestamp, nonce, "", signature);
                decryptMsg = postStr;
            }
            else
            {
                // 密文验签
                ret = QyCrypt.DecryptMsg(msg_signature, timestamp, nonce, postStr, ref decryptMsg);
            }

            if (ret != 0)
            {
                // 验签、解密失败
                logHelper.Error("Process：微信通知消息验证失败：错误代码：" + ret);
                // 返回Sucess防止微信后续5次的推送（未收到Success之前都会一直推送）
                return wxAutoComResponse.ResponseOK(); ;
            }

            logHelper.Debug("Process:解密后内容:" + decryptMsg);
            // 序列化接收到的消息
            PubReceiveMsgCData recMsg = ComHelper.XmlDeserialize<PubReceiveMsgCData>(decryptMsg);
            logHelper.Debug("ReceiveMsgCData:" + recMsg.JsonSerialize());

            // 保存日志到数据库
            await wxDbLogHelper.SaveWxOptLog(recMsg);
            // 保存用户信息
            await wxDbLogHelper.SaveWxUser(recMsg);

            // 判断是否接入客服
            var kfRes = wxKfTransferHelper.ChatWithKf(recMsg);
            if (kfRes.IsSucc)
            {
                return await wxAutoComResponse.AutoMsgResponse(kfRes.Data);
            }

            // 判断消息类型
            switch (recMsg.MsgType)
            {
                case PubMsgType.text:
                    logHelper.Debug("text");
                    return textHandler.DealQyText(recMsg);
                case PubMsgType.image:
                    logHelper.Debug("image");
                    break;
                case PubMsgType.voice:
                    logHelper.Debug("voice");
                    break;
                case PubMsgType.video:
                    logHelper.Debug("video");
                    break;
                case PubMsgType.shortvideo:
                    logHelper.Debug("shortvideo");
                    break;
                case PubMsgType.location:
                    logHelper.Debug("location");
                    break;
                case PubMsgType.link:
                    logHelper.Debug("link");
                    break;
                case PubMsgType.@event:
                    logHelper.Debug("event");
                    return eventHandler.DealQyEvent(recMsg);

                //case PubMsgType.wxcard:
                //    logHelper.Debug("wxcard");
                //    break;
                //case PubMsgType.mpnews:
                //    logHelper.Debug("mpnews");
                //    break;
                //case PubMsgType.news:
                //    logHelper.Debug("news");
                //    break;
                //case PubMsgType.music:
                //    logHelper.Debug("music");
                //    break;
                default:
                    break;
            }

            return wxAutoComResponse.ResponseOK(); ;
        }
    }
}