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
    /// 微信客服转接消息帮助类
    /// </summary>
    public class WxKfTransferHelper
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxKfTransferHelper> logHelper;

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly ICacheHelper cacheHelper;

        /// <summary>
        /// 微信自动通用应答
        /// </summary>
        private readonly WxAutoComResponse wxAutoComResponse;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="repo"></param>
        /// <param name="cacheHelper"></param>
        /// <param name="wxAutoComResponse"></param>
        public WxKfTransferHelper(ILogHelper<WxKfTransferHelper> logHelper, IRepository repo, ICacheHelper cacheHelper,
            WxAutoComResponse wxAutoComResponse)
        {
            this.logHelper = logHelper;
            this.repo = repo;
            this.cacheHelper = cacheHelper;
            this.wxAutoComResponse = wxAutoComResponse;
        }

        /// <summary>
        /// 进入客服的关键字
        /// </summary>
        private List<string> LstKfKeywords
        {
            get
            {
                var strKeys = cacheHelper.GetDbConfValByKey("kfkeywords");
                if (!string.IsNullOrEmpty(strKeys))
                {
                    var lstKeys = strKeys.Split(new string[] { ",", "、" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    return lstKeys;
                }

                return new List<string> { "人工", "客服", "小美", "小联", "gotoservices", "GoToServices" };
            }
        }

        /// <summary>
        /// 是否工作时间
        /// </summary>
        /// <returns></returns>
        public bool IsWorkTime()
        {
            return true;
            //try
            //{
            //    DateTime stime = DateTime.Now;
            //    switch (stime.DayOfWeek)
            //    {
            //        case DayOfWeek.Monday:
            //        case DayOfWeek.Thursday:
            //        case DayOfWeek.Tuesday:
            //        case DayOfWeek.Wednesday:
            //        case DayOfWeek.Friday:
            //            if (stime.Hour < 9 || stime.Hour > 18 || stime.Hour == 12)
            //            {
            //                return false;
            //            }
            //            break;
            //        case DayOfWeek.Saturday:
            //        case DayOfWeek.Sunday:
            //            return false;
            //        default:
            //            break;
            //    }
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    logHelper.Log("IsWorkTime出错：" + ex.Message + "        " + ex.StackTrace);
            //    return false;
            //}
        }

        /// <summary>
        /// 5分钟内有沟通记录才转发客服
        /// </summary>
        /// <returns></returns>
        public bool IsIn5Min(string openId)
        {
            var inTime = DateTime.Now.AddMinutes(-5);
            var sql = $@" SELECT COUNT(1) FROM WxUserOptLog 
WHERE FromUserName = @openId
AND CreateTime > @inTime
AND ((MsgType = 'event' AND Event = 'click' AND EventKey = 'gotoservices') OR (MsgType = 'text' AND Content IN (@LstKfKeywords))) ";
            //var chatCount = repo.GetScalar<long>(sql, new { openId, inTime, LstKfKeywords });
            var chatCount = repo.SqlQuerySingle<int>(sql, new { openId, inTime, LstKfKeywords });
            if (chatCount > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断是否客服消息
        /// 如果不是事件消息 或者 事件key是联系客服 或者 回复的文本是客服关键字 都属于联系客服
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        public bool IsKfMsg(PubReceiveMsgCData recMsg)
        {
            var res = recMsg.MsgType.ToString().ToLower() != "event"
                || (!string.IsNullOrEmpty(recMsg.EventKey) && recMsg.EventKey.ToLower() == "gotoservices")
                || (!string.IsNullOrEmpty(recMsg.Content) && LstKfKeywords.Contains(recMsg.Content));
            return res;
        }

        /// <summary>
        /// 联系客服
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        public Result<string> ChatWithKf(PubReceiveMsgCData recMsg)
        {
            try
            {
                if (IsWorkTime() && IsIn5Min(recMsg.FromUserName) && IsKfMsg(recMsg))
                {
                    PubKfApi.SendTextMsg(recMsg.FromUserName, "正在为你转接在线客服，请稍后.....");
                    var msg = PubMsgApi.BuildKfTransferMsg(recMsg.FromUserName, recMsg.ToUserName, recMsg.CreateTime);
                    return new Result<string> { IsSucc = true, Data = msg };
                }
            }
            catch (Exception ex)
            {
                logHelper.Error("ChatWithKf:联系客服失败：" + ex.Message + "     " + ex.StackTrace);
            }

            return new Result<string> { Message = "联系客服失败！" };
        }

        /// <summary>
        /// 联系客服(菜单)
        /// </summary>
        /// <param name="recMsg"></param>
        /// <returns></returns>
        public async Task<string> ContactKf(PubReceiveMsg recMsg)
        {
            try
            {
                var timeStamp = ComHelper.ConvertDateTimeInt(DateTime.Now);
                if (!IsWorkTime())
                {
                    var msg = "您好！现在非客服上班时间，请您在每天9-22点联系我们的客服美眉！";
                    return await wxAutoComResponse.SendWxText(recMsg.FromUserName, recMsg.ToUserName, timeStamp, msg);
                }
                else
                {
                    PubKfApi.SendTextMsg(recMsg.FromUserName, "正在为你转接在线客服，请稍后.....");
                    var msg = PubMsgApi.BuildKfTransferMsg(recMsg.FromUserName, recMsg.ToUserName, timeStamp);
                    logHelper.Debug("ContactKf:msg:" + msg.JsonSerialize());
                    // 发送客服消息提醒
                    return await wxAutoComResponse.AutoMsgResponse(msg);
                }
            }
            catch (Exception ex)
            {
                logHelper.Error("ContactKf:联系客服失败:" + ex.Message + "        " + ex.StackTrace);
            }

            return wxAutoComResponse.ResponseOK(); ;
        }
    }
}
