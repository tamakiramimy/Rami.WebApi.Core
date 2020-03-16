using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 授权定义
    /// </summary>
    public class AuthConst
    {
        /// <summary>
        /// Mvc授权-简单
        /// </summary>
        public const string MvcAuthSimple = "MvcAuthSimple";

        /// <summary>
        /// Mvc授权
        /// </summary>
        public const string MvcAuthReq = "MvcAuthReq";

        /// <summary>
        /// Mvc授权(跳转界面为/Admin/Login、角色要求Admin)
        /// </summary>
        public const string MvcAuthReqAdmin = "MvcAuthReqAdmin";

        /// <summary>
        /// API授权-简单
        /// </summary>
        public const string ApiAuthSimple = "ApiAuthSimple";

        /// <summary>
        /// API授权
        /// </summary>
        public const string ApiAuthReq = "ApiAuthReq";

        /// <summary>
        /// API授权（角色要求Admin，验证JWT颁发者）
        /// </summary>
        public const string ApiAuthReqAdmin = "ApiAuthReqAdmin";
    }
}
