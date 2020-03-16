using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    [SqlSugar.SugarTable("PermMenu")]
    public class PermMenu : BaseDbShow
    {
        /// <summary>
        /// Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int PId { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        public string Router { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrdInd { get; set; }

        /// <summary>
        /// 树代码
        /// </summary>
        public string TreeCode { get; set; }

        /// <summary>
        /// 菜单样式
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string IconClass { get; set; }

        /// <summary>
        /// 是否需要授权(0:否;1:是)
        /// </summary>
        public int NeedAuth { get; set; }

        /// <summary>
        /// 是否隐藏菜单
        /// </summary>
        public int IsHide { get; set; }

        /// <summary>
        /// 是否Url链接
        /// </summary>
        public int IsUrl { get; set; }
    }

    /// <summary>
    /// 菜单Dto
    /// </summary>
    public class PermMenuDto : PermMenu
    {
        /// <summary>
        /// 是否需要授权(0:否;1:是)
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string NeedAuthShow
        {
            get
            {
                return NeedAuth == 1 ? "需要" : "不需要";
            }
        }

        /// <summary>
        /// 父节点递归
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<int> PIdArr { get; set; }

        /// <summary>
        /// 全路径
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string FullPath { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string IsHideShow
        {
            get
            {
                return IsHide == 1 ? "是" : "否";
            }
        }
    }

    /// <summary>
    /// 菜单路由vue router
    /// </summary>
    public class NavigationBar
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        public int pid { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int order { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 路由Meta
        /// </summary>
        public NavigationBarMeta meta { get; set; }

        /// <summary>
        /// 子路由
        /// </summary>
        public List<NavigationBar> children { get; set; }

        /// <summary>
        /// 菜单样式
        /// </summary>
        public string iconCls { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHide { get; set; } = false;

        /// <summary>
        /// 是否按钮控制权限(暂时没用)
        /// </summary>
        public bool IsButton { get; set; } = false;

        /// <summary>
        /// 按钮对应API权限控制(暂时没用)
        /// </summary>
        public string Func { get; set; }

        /// <summary>
        /// 是否Url链接
        /// </summary>
        public bool IsUrl { get; set; }
    }

    /// <summary>
    /// 菜单数据
    /// </summary>
    public class NavigationBarMeta
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 是否需要授权
        /// </summary>
        public bool requireAuth { get; set; } = true;

        /// <summary>
        /// 非Tab页签
        /// </summary>
        public bool NoTabPage { get; set; } = false;
    }

    /// <summary>
    /// 菜单树
    /// </summary>
    public class PermissionTree
    {
        /// <summary>
        /// Id
        /// </summary>
        public int value { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int order { get; set; }

        /// <summary>
        /// 是否按钮
        /// </summary>
        public bool isbtn { get; set; }

        /// <summary>
        /// 是否禁止选择
        /// </summary>
        public bool disabled { get; set; }

        /// <summary>
        /// 值菜单
        /// </summary>
        public List<PermissionTree> children { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        public List<PermissionTree> btns { get; set; }
    }
}
