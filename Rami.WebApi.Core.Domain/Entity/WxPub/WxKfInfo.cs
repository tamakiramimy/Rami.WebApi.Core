using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 微信客服账号
    /// </summary>
    [SqlSugar.SugarTable("WxKfInfo")]
    public class WxKfInfo : BaseDbShow
    {
        /// <summary>
        /// 完整客服帐号，格式为：帐号前缀@公众号微信号
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string KfAccount { get; set; }

        /// <summary>
        /// 客服昵称
        /// </summary>
        public string KfNick { get; set; }

        /// <summary>
        ///  客服编号
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string KfId { get; set; }

        /// <summary>
        /// 客服头像
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string KfHeadimgurl { get; set; }

        /// <summary>
        /// 客服头像上传的虚拟路径
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string KfHeadUpVir { get; set; }

        /// <summary>
        /// 如果客服帐号已绑定了客服人员微信号， 则此处显示微信号
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string KfWx { get; set; }

        /// <summary>
        /// 如果客服帐号尚未绑定微信号，但是已经发起了一个绑定邀请， 则此处显示绑定邀请的微信号
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string InviteWx { get; set; }

        /// <summary>
        /// 如果客服帐号尚未绑定微信号，但是已经发起过一个绑定邀请， 邀请的过期时间，为unix 时间戳
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public int? InviteExpireTime { get; set; }

        /// <summary>
        /// 邀请的状态，有等待确认“waiting”，被拒绝“rejected”， 过期“expired”
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string InviteStatus { get; set; }
    }

    /// <summary>
    /// 微信客服账号
    /// </summary>
    public class WxKfInfoShow : WxKfInfo
    {
        /// <summary>
        /// 客服头像
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string KfHeadUpShow { get; set; }
    }
}
