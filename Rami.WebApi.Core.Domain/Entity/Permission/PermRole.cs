using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 用户角色
    /// </summary>
    [SqlSugar.SugarTable("PermRole")]
    public class PermRole : BaseDbShow
    {
        /// <summary>
        /// Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrdInd { get; set; }
    }

    /// <summary>
    /// 用户角色
    /// </summary>
    public class PermRoleDto : PermRole
    {
    }
}
