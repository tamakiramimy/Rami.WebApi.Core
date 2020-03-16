using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 定时任务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TimingJobController : ControllerBase
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<TimingJobController> logHelper;

        /// <summary>
        /// 自动推送服务
        /// </summary>
        private readonly AutoPushSvc autoPushSvc;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="autoPushSvc"></param>
        public TimingJobController(ILogHelper<TimingJobController> logHelper, AutoPushSvc autoPushSvc)
        {
            this.logHelper = logHelper;
            this.autoPushSvc = autoPushSvc;
        }

        /// <summary>
        /// 自动推送
        /// </summary>
        /// <returns></returns>
        [HttpGet("StartSend")]
        public async Task<Result> StartSend()
        {
            return await autoPushSvc.TimingPush();
        }
    }
}