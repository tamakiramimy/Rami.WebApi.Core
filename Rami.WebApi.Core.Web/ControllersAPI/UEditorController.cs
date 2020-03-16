using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UEditorNetCore;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// UEditor API
    /// </summary>
    public class UEditorController : BaseController
    {
        /// <summary>
        /// UE
        /// </summary>
        private readonly UEditorService ue;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ue"></param>
        public UEditorController(UEditorService ue)
        {
            this.ue = ue;
        }

        /// <summary>
        /// 处理
        /// </summary>
        [HttpGet]
        [HttpPost]
        public void DealUeAction()
        {
            ue.DoAction(HttpContext);
        }
    }
}