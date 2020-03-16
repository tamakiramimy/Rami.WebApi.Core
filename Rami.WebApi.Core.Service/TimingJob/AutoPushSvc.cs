using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.Wechat.Core.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 自动推送定时服务
    /// </summary>
    public class AutoPushSvc
    {
        /// <summary>
        /// 定时服务间隔（每隔多少分钟执行一次）
        /// </summary>
        private const int TimingSpanMin = 10;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<AutoPushSvc> logHelper;

        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 微信消息转换帮助类
        /// </summary>
        private readonly WxAutoConvertHelper wxAutoConvertHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="repo"></param>
        /// <param name="wxAutoConvertHelper"></param>
        public AutoPushSvc(ILogHelper<AutoPushSvc> logHelper, IRepository repo, WxAutoConvertHelper wxAutoConvertHelper)
        {
            this.logHelper = logHelper;
            this.repo = repo;
            this.wxAutoConvertHelper = wxAutoConvertHelper;
        }

        /// <summary>
        /// 定时推送
        /// </summary>
        /// <returns></returns>
        public async Task<Result> TimingPush()
        {
            // todo改成多线程推送
            var dtNow = DateTime.Now;
            // 获取当前时间间隔在1分钟之内的任务
            var lstPushConf = await repo.QueryAsync<WxAutoPushShow>(x => x.IsDel == 0 && x.PushTime >= dtNow.AddMinutes(-TimingSpanMin) && x.PushTime < dtNow.AddMinutes(TimingSpanMin));
            foreach (var push in lstPushConf)
            {
                try
                {
                    // 查询推送用户列表
                    logHelper.Debug("AutoPushJob:任务信息:" + ComHelper.JsonSerialize(push));
                    string strSql = Get2DayActWxUserSql(push);
                    var lstWxUser = await repo.SqlQueryAsync<WxUserInfo>(strSql, new { CreateTime = dtNow.AddDays(-2) });
                    logHelper.Debug("AutoPushJob:符合任务ID：" + push.Id + "任务名：" + push.PushName + "的用户有：" + lstWxUser.Select(x => x.OpenId).Distinct().JsonSerializeNoNull());

                    // 推送消息
                    await PushMsgToUser(push, lstWxUser);
                }
                catch (Exception ex)
                {
                    logHelper.Error("AutoPushJob:获取定时推送消息失败：" + ex.Message + "      " + ex.StackTrace);
                }
            }

            return new Result { IsSucc = true, Message = "定时推送任务执行完成！" };
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="push"></param>
        /// <param name="lstWxUser"></param>
        /// <returns></returns>
        private async Task PushMsgToUser(WxAutoPushShow push, List<WxUserInfo> lstWxUser)
        {
            // 构造消息
            var baseMsg = new PubKfBaseMsg();
            var apMsgType = ComHelper.GetEnumValueByStr<Enum_ApMsgType>(push.ContentType);
            // 构造自动推送消息
            baseMsg = await GetAutoPushMsg(push, baseMsg, apMsgType);
            logHelper.Debug("AutoPushJob:即将发送的消息为：" + baseMsg.JsonSerializeNoNull());
            // 遍历发送客服消息
            foreach (var user in lstWxUser)
            {
                try
                {
                    await SendAutoPushMsg(push, baseMsg, apMsgType, user);
                }
                catch (Exception ex)
                {
                    logHelper.Error("AutoPushJob:发送消息给用户失败：" + user.OpenId + "      " + ex.Message + "      " + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 获取2天内有交互的微信用户
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string Get2DayActWxUserSql(WxAutoPush item)
        {
            //  48小时内最后一次操作记录不是定位不是取消关注的用户信息
            var strSql = @" SELECT OpenId ,Nickname FROM WxUserInfo u
	   WHERE u.IsSubscribe = 1
	   AND EXISTS 
	   (SELECT 1 FROM 
		(
				SELECT RANK() OVER (PARTITION BY FromUserName ORDER BY CreateTime DESC) AS Rn,Id,FromUserName,Event
				 FROM WxUserOptLog
				WHERE CreateTime > @CreateTime
		) AS tb1
		WHERE tb1.Rn = 1
		AND tb1.Event <> 'unsubscribe'
		AND u.OpenId = tb1.FromUserName) 
{0} ";
            var strWhere = string.Empty;

            // 国家
            if (!string.IsNullOrEmpty(item.Country))
            {
                strWhere += string.Format(" AND u.Country = '{0}' ", item.Country);
            }
            // 省
            if (!string.IsNullOrEmpty(item.Province))
            {
                strWhere += string.Format(" AND u.Province = '{0}' ", item.Province);
            }
            // 市
            if (!string.IsNullOrEmpty(item.City))
            {
                strWhere += string.Format(" AND u.City = '{0}' ", item.City);
            }
            // 性别
            if (!string.IsNullOrEmpty(item.PushingSex))
            {
                strWhere += string.Format(" AND u.Sex IN ({0}) ", item.PushingSex);
            }
            // 回复过关键字
            if (!string.IsNullOrEmpty(item.OldKeyWords))
            {
                strWhere += string.Format(" AND EXISTS (SELECT 1 FROM ChatUsersMessage his WHERE his.FromUserName = u.OpenId AND his.Content = '{0}') ", item.OldKeyWords);
            }

            // 拼凑sql
            strSql = string.Format(strSql, strWhere);
            return strSql;
        }

        /// <summary>
        /// 构造自动推送消息
        /// </summary>
        /// <param name="push"></param>
        /// <param name="baseMsg"></param>
        /// <param name="apMsgType"></param>
        /// <returns></returns>
        private async Task<PubKfBaseMsg> GetAutoPushMsg(WxAutoPushShow push, PubKfBaseMsg baseMsg, Enum_ApMsgType apMsgType)
        {
            switch (apMsgType)
            {
                // 文本(昵称)和红包消息需要单独发送
                case Enum_ApMsgType.Image:
                    baseMsg = new PubKfImgMsg { image = new PubKfImgContent { media_id = push.MediaId } };
                    break;
                case Enum_ApMsgType.Voice:
                    baseMsg = new PubKfVoiceMsg { voice = new PubKfVoiceContent { media_id = push.MediaId } };
                    break;
                case Enum_ApMsgType.Video:
                    baseMsg = new PubKfVideoMsg { video = new PubKfVideoContent { media_id = push.MediaId, thumb_media_id = push.VideoThumbMediaId, title = push.VideoTitle, description = push.VideoDescription } };
                    break;
                case Enum_ApMsgType.News:
                    baseMsg = new PubKfMpNewsMsg { mpnews = new PubKfMpnewsContent { media_id = push.MediaId } };
                    break;
                case Enum_ApMsgType.BackNews:
                    var lstArts = await wxAutoConvertHelper.GetAutoPushBackNews(push);
                    baseMsg = new PubKfNewsMsg { news = new PubKfNewsContent { articles = lstArts } };
                    break;
                default:
                    break;
            }

            return baseMsg;
        }

        /// <summary>
        /// 定时推送消息
        /// </summary>
        /// <param name="push"></param>
        /// <param name="baseMsg"></param>
        /// <param name="apMsgType"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task SendAutoPushMsg(WxAutoPushShow push, PubKfBaseMsg baseMsg, Enum_ApMsgType apMsgType, WxUserInfo user)
        {
            // 文本消息 昵称赋值
            if (apMsgType == Enum_ApMsgType.Text)
            {
                var txt = push.TextContent.Replace("{nickname}", user.NickName);
                baseMsg = new PubKfTextMsg { text = new PubKfTextContent { content = txt } };
            }

            // 红包单独发送
            if (apMsgType == Enum_ApMsgType.RedBag)
            {
                var apiRes = await wxAutoConvertHelper.SendAutoPushRedPack(push, user.OpenId);
                logHelper.Debug("AutoPushJob:发送红包用户：" + user.OpenId + "     结果:" + apiRes);
            }
            else
            {
                // 非红包消息发送
                var apiRes = PubKfApi.SendMsg(baseMsg, user.OpenId);
                if (apiRes.IsSuss)
                {
                    await wxAutoConvertHelper.SaveAutoPushHis(push.Id, user.OpenId);
                }
                else
                {
                    logHelper.Error("AutoPushJob:发送消息给" + user.OpenId + "失败：" + apiRes.errcode + "      " + apiRes.errmsg);
                }

                logHelper.Debug("AutoPushJob:发送消息用户：" + user.OpenId + "     结果:" + apiRes.JsonSerialize());
            }
        }
    }
}
