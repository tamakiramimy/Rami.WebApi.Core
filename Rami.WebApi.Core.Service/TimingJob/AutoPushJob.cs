using Quartz;
using Rami.WebApi.Core.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rami.WebApi.Core.Service
{
    /// <summary>
    /// 自动推送服务
    /// （在 Program写死调用，不推荐，建议用定时服务后台管理平台，通过Webapi方式调用）
    /// </summary>
    public class AutoPushJob : IJob
    {
        /// <summary>
        /// 服务
        /// </summary>
        private IServiceProvider serviceProvider;

        /// <summary>
        /// 日志
        /// </summary>
        private ILogHelper<AutoPushJob> logHelper;

        /// <summary>
        /// 自动推送服务
        /// </summary>
        private AutoPushSvc autoPushSvc;

        ///// <summary>
        ///// 构造方法
        ///// </summary>
        ///// <param name="logHelper"></param>
        ///// <param name="autoPushSvc"></param>
        //public AutoPushJob(ILogHelper<AutoPushJob> logHelper, AutoPushSvc autoPushSvc)
        //{
        //    this.logHelper = logHelper;
        //    this.autoPushSvc = autoPushSvc;
        //}

        /// <summary>
        /// 执行定时任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            this.serviceProvider = StaticServiceProvider.Current;
            this.logHelper = serviceProvider.GetRequiredService<ILogHelper<AutoPushJob>>();
            this.autoPushSvc = serviceProvider.GetRequiredService<AutoPushSvc>();

            logHelper.Debug($"AutoPushJob:微信定时推送任务开始执行：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ");
            await autoPushSvc.TimingPush();
            logHelper.Debug($"AutoPushJob:微信定时推送任务执行结束：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ");
        }
    }
}
