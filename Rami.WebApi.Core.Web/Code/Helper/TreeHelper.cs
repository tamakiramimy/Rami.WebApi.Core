using Rami.WebApi.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 树帮助类
    /// </summary>
    public class TreeHelper
    {
        /// <summary>
        /// 递归生成树
        /// </summary>
        /// <param name="lstAllNode"></param>
        /// <param name="curNode"></param>
        public static void LoopNaviBarAppendChildren(List<NavigationBar> lstAllNode, NavigationBar curNode)
        {
            var lstChilNodes = lstAllNode.Where(x => x.pid == curNode.id).ToList();
            if (lstChilNodes.Count > 0)
            {
                curNode.children = new List<NavigationBar>();
                curNode.children.AddRange(lstChilNodes);
            }
            else
            {
                curNode.children = null;
            }

            foreach (var childNode in lstChilNodes)
            {
                LoopNaviBarAppendChildren(lstAllNode, childNode);
            }
        }

        /// <summary>
        /// 菜单树
        /// </summary>
        /// <param name="lstAllNode"></param>
        /// <param name="curNode"></param>
        public static void LoopMenuTreeAppendChildren(List<PermissionTree> lstAllNode, PermissionTree curNode)
        {
            var lstChildNodes = lstAllNode.Where(x => x.Pid == curNode.value).ToList();
            if (lstChildNodes.Count > 0)
            {
                curNode.children = new List<PermissionTree>();
                curNode.children.AddRange(lstChildNodes);
            }
            else
            {
                curNode.children = null;
            }

            foreach (var childNode in lstChildNodes)
            {
                LoopMenuTreeAppendChildren(lstAllNode, childNode);
            }
        }
    }
}
