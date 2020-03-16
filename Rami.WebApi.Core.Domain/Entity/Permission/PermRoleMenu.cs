using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 角色-菜单关系
    /// </summary>
    [SqlSugar.SugarTable("PermRoleMenu")]
    public class PermRoleMenu : BaseDbShow
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int RId { get; set; }

        /// <summary>
        /// 菜单Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int MId { get; set; }
    }

    /// <summary>
    /// 角色-菜单关系
    /// </summary>
    public class PermRoleMenuDto : PermRoleMenu
    {
    }

    /// <summary>
    /// 角色-菜单关系 展示
    /// </summary>
    public class PermRoleMenuVto
    {

    }
}
