using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 通用微信用户
    /// </summary>
    [SqlSugar.SugarTable("WxUserInfo")]
    public class WxUserInfo : BaseDbShow
    {
        /// <summary>
        /// 微信Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public string OpenId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Country { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string City { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Privilege { get; set; }

        /// <summary>
        /// 关联公众号ID（多个公众号OpenID互转）
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Unionid { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Language { get; set; }

        /// <summary>
        /// 公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 用户所在的分组ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int Groupid { get; set; }

        /// <summary>
        /// 是否关注(0:未关注;1:关注)
        /// </summary>
        public int IsSubscribe { get; set; }

        /// <summary>
        /// 关注时间
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime? SubscribeTime { get; set; }
    }
}
