using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 用户-角色关系表
    /// </summary>
    [SqlSugar.SugarTable("PermUserRole")]
    public class PermUserRole : BaseDbShow
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string UserName { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int RId { get; set; }
    }

    /// <summary>
    /// 用户-角色关系表
    /// </summary>
    public class PermUserRoleDto : PermUserRole
    {
    }

    /// <summary>
    /// 用户-角色关系 展示
    /// </summary>
    public class PermUserRoleVto
    {
        /// <summary>
        /// oa账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色Ids
        /// </summary>
        public string RIds { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Descriptions { get; set; }

        /// <summary>
        /// 角色Id列表
        /// </summary>
        public List<int> LstRIds{ get; set; }
    }
}
