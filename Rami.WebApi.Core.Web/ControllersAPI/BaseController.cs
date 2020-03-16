using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// API父类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
    }
}