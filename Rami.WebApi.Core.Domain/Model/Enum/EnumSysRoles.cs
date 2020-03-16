using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public enum EnumSysRoles
    {
        /// <summary>
        /// 管理员
        /// </summary>
        [Description("超级管理员")]
        SuperAdmin = 1,

        /// <summary>
        /// 管理员
        /// </summary>
        [Description("管理员")]
        WxAdmin = 2,

        /// <summary>
        /// 微信客服
        /// </summary>
        [Description("微信客服")]
        WxKF = 3,
    }
}
