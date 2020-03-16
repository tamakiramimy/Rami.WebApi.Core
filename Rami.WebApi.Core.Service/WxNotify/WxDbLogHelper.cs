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
    /// 微信数据库日志帮助类
    /// </summary>
    public class WxDbLogHelper
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<WxDbLogHelper> logHelper;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="logHelper"></param>
        public WxDbLogHelper(IRepository repo, ILogHelper<WxDbLogHelper> logHelper)
        {
            this.repo = repo;
            this.logHelper = logHelper;
        }

        /// <summary>
        /// 保存微信日志到数据库
        /// </summary>
        /// <returns></returns>
        public async Task SaveWxOptLog(PubReceiveMsgCData recMsg)
        {
            try
            {
                // 判断是否重复提交
                var exist = await repo.FirstOrDefaultAsync<WxUserOptLog>(d => d.FromUserName == recMsg.FromUserName && d.CreateTime == recMsg.DtCreateTime);
                if (exist != null)
                {
                    logHelper.Debug("重复的微信请求（微信首次请求我方服务器已保存记录但未返回正确回应的二次请求），不保存！IopenId:" + recMsg.FromUserName);
                    return;
                }

                // 保存微信日志
                var wxLog = new WxUserOptLog();
                wxLog.MsgId = recMsg.MsgId.ToString();
                wxLog.ToUserName = recMsg.ToUserName;
                wxLog.FromUserName = recMsg.FromUserName;
                wxLog.CreateTime = recMsg.DtCreateTime.Value;
                wxLog.MsgType = recMsg.MsgType.ToString();
                wxLog.Content = recMsg.Content;
                wxLog.PicUrl = recMsg.PicUrl;
                wxLog.MediaId = recMsg.MediaId;
                wxLog.Format = recMsg.Format;
                wxLog.ThumbMediaId = recMsg.ThumbMediaId;
                wxLog.LocationX_Latitude = recMsg.Location_X.ToString();
                wxLog.LocationY_Longitude = recMsg.Location_Y.ToString();
                wxLog.Scale = recMsg.Scale.ToString();
                wxLog.LabelPrecision = recMsg.Label;
                wxLog.Title = recMsg.Title;
                wxLog.Description = recMsg.Description;
                wxLog.Url = recMsg.Url;
                wxLog.Event = recMsg.Event.ToString();
                wxLog.EventKey = recMsg.EventKey;
                wxLog.Ticket = recMsg.Ticket;

                wxLog.Creater = "system";
                wxLog.CreateTime = recMsg.DtCreateTime.Value;
                wxLog.Updater = "system";
                wxLog.UpdateTime = recMsg.DtCreateTime.Value;
                await repo.SaveAsync(wxLog);
                logHelper.Debug("SaveWxDbLog:保存微信日志到数据成功：" + ComHelper.JsonSerialize(wxLog));
            }
            catch (Exception ex)
            {
                logHelper.Error("SaveWxDbLog:保存微信日志到数据失败：" + ex.Message + "     " + ex.StackTrace);
            }
        }

        /// <summary>
        /// 保存微信用户信息
        /// </summary>
        /// <param name="recMsg"></param>
        public async Task SaveWxUser(PubReceiveMsgCData recMsg)
        {
            try
            {
                var exist = await repo.FirstOrDefaultAsync<WxUserInfo>(x => x.OpenId == recMsg.FromUserName);
                // 只要用户不存在，且不是取消关注就拉取用户信息
                var isUnsubscribe = recMsg.Event.HasValue && recMsg.Event == PubEventType.unsubscribe;
                if (exist == null)
                {
                    if (!isUnsubscribe)
                    {
                        // 用户信息不存在
                        var user = PubUserApi.GetUserinfo(recMsg.FromUserName);
                        if (!user.IsSuss)
                        {
                            logHelper.Error("SaveWxUser:保存微信用户信息失败代码：" + user.errcode + "        错误信息:" + user.errmsg);
                        }

                        logHelper.Debug("SaveWxUser:获取到微信用户信息：" + user.JsonSerializeNoNull());
                        // 保存用户信息
                        var wxUser = new WxUserInfo();
                        wxUser.OpenId = user.openid;
                        wxUser.NickName = user.nickname;
                        wxUser.HeadImgUrl = user.headimgurl;
                        wxUser.Sex = (short)user.sex;
                        wxUser.Country = user.country;
                        wxUser.Province = user.province;
                        wxUser.City = user.city;
                        wxUser.Unionid = user.unionid;
                        wxUser.Language = user.language;
                        wxUser.Remark = user.remark;
                        wxUser.Groupid = user.groupid;
                        wxUser.IsSubscribe = 1;
                        wxUser.SubscribeTime = ComHelper.ConvertTimeStampToDate(user.subscribe_time);

                        if (user.privilege != null && user.privilege.Count > 0)
                        {
                            wxUser.Privilege = string.Join("',", user.privilege);
                        }

                        wxUser.Creater = "system";
                        wxUser.CreateTime = recMsg.DtCreateTime.Value;
                        wxUser.Updater = "system";
                        wxUser.UpdateTime = recMsg.DtCreateTime.Value;
                        await repo.SaveAsync(wxUser);
                    }
                }
                else if (exist != null && !isUnsubscribe && exist.IsSubscribe == 0)
                {
                    // 重新关注修改状态
                    exist.IsSubscribe = 1;
                    if (!exist.SubscribeTime.HasValue)
                    {
                        exist.SubscribeTime = recMsg.DtCreateTime;
                    }

                    await repo.UpdateAsync(exist, new List<string> { "IsSubscribe", "SubscribeTime" });
                }
                else if (exist != null && isUnsubscribe)
                {
                    logHelper.Debug("SaveWxUser:用户：" + exist.OpenId + "于" + DateTime.Now.ToString("yyyyMMddHHmmss") + "取消关注！");
                    // 用户取消关注
                    exist.IsSubscribe = 0;
                    await repo.UpdateAsync(exist, new List<string> { "IsSubscribe" });
                }
            }
            catch (Exception ex)
            {
                logHelper.Error("SaveWxUser:保存微信用户信息失败：" + ex.Message + "     " + ex.StackTrace);
            }
        }

        /// <summary>
        /// 微信关注/取关记录
        /// </summary>
        /// <param name="recMsg"></param>
        /// <param name="type">0:关注;1:取关</param>
        public async Task SaveSubLog(PubReceiveMsgCData recMsg, int type)
        {
            try
            {
                // 增加关注记录
                var subLog = new WxSubScribeLog();
                subLog.OpenId = recMsg.FromUserName;
                subLog.SubScribeType = type;
                subLog.OptDate = recMsg.DtCreateTime.Value;
                subLog.QrSceneStr = recMsg.EventKey;
                subLog.QrTicketId = recMsg.Ticket;

                subLog.Creater = "system";
                subLog.CreateTime = recMsg.DtCreateTime.Value;
                subLog.Updater = "system";
                subLog.UpdateTime = recMsg.DtCreateTime.Value;
                await repo.SaveAsync(subLog);
            }
            catch (Exception ex)
            {
                logHelper.Error("DealWxSubscribe:扫码关注失败：" + ex.Message + "     " + ex.StackTrace);
            }
        }

        /// <summary>
        /// 微信扫码记录
        /// </summary>
        /// <param name="recMsg"></param>
        public async Task SaveQrLog(PubReceiveMsgCData recMsg)
        {
            try
            {
                // 增加扫码记录
                if (recMsg.Event == PubEventType.subscribe || recMsg.Event == PubEventType.scan)
                {
                    var qrLog = new WxQrCodeLog();
                    qrLog.OpenId = recMsg.FromUserName;
                    qrLog.ScanDate = recMsg.DtCreateTime.Value;
                    qrLog.QrSceneStr = recMsg.EventKey;
                    qrLog.QrTicketId = recMsg.Ticket;

                    qrLog.Creater = "system";
                    qrLog.CreateTime = recMsg.DtCreateTime.Value;
                    qrLog.Updater = "system";
                    qrLog.UpdateTime = recMsg.DtCreateTime.Value;
                    await repo.SaveAsync(qrLog);
                }
            }
            catch (Exception ex)
            {
                logHelper.Error("DealWxSubscribe:扫码关注失败：" + ex.Message + "     " + ex.StackTrace);
            }
        }
    }
}
