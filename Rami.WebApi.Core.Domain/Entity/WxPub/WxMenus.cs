using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Domain
{
    /// <summary>
    /// 微信公众号菜单
    /// </summary>
    [SqlSugar.SugarTable("WxMenus")]
    public class WxMenus : BaseDbShow
    {
        /// <summary>
        /// Id
        /// </summary>
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 父菜单
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 级别(一级菜单、二级菜单)
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过60个字节
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// 菜单的响应动作类型，view表示网页类型，click表示点击类型，miniprogram表示小程序类型
        /// (枚举：WeChat.Public.MenuType)
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Type { get; set; }

        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Key { get; set; }

        /// <summary>
        /// 网页 链接，用户点击菜单可打开链接，不超过1024字节。 type为miniprogram时，不支持小程序的老版本客户端将打开本url
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Url { get; set; }

        /// <summary>
        /// 调用新增永久素材接口返回的合法media_id
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string MediaId { get; set; }

        /// <summary>
        /// 小程序的appid（仅认证公众号可配置）
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Appid { get; set; }

        /// <summary>
        /// 小程序的页面路径
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public string Pagepath { get; set; }
    }

    /// <summary>
    /// 微信公众号菜单
    /// </summary>
    public class WxMenusShow : WxMenus
    {
        /// <summary>
        /// 二级菜单数组，个数应为1~5个
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<WxMenusShow> SubButton { get; set; }
    }
}
