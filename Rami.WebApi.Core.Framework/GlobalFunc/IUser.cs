using System.Collections.Generic;
using System.Security.Claims;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 全局用户接口
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 姓名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 角色列表
        /// </summary>
        List<string> LstRoles { get; }

        /// <summary>
        /// 角色
        /// </summary>
        string Roles { get; }

        /// <summary>
        /// ID
        /// </summary>
        int ID { get; }

        /// <summary>
        /// 是否授权
        /// </summary>
        /// <returns></returns>
        bool IsAuthenticated();

        /// <summary>
        /// 用户认证 Claim
        /// </summary>
        /// <returns></returns>
        IEnumerable<Claim> GetClaimsIdentity();

        /// <summary>
        /// 根据 Claim 获取值
        /// </summary>
        /// <param name="ClaimType"></param>
        /// <returns></returns>
        List<string> GetClaimValueByType(string ClaimType);
    }
}
