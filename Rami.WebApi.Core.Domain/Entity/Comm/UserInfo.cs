using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [SqlSugar.SugarTable("UserInfo")]
    public class UserInfo : BaseDbShow
    {
        /// <summary>
        /// Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPwd { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Email { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Phone { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string UserIcon { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoShow : UserInfo
    {
    }
}
