using Autofac.Extensions.DependencyInjection;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Rami.WebApi.Core.Web
{
    /// <summary>
    /// 程序入口
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 任务列表
        /// </summary>
        private static IScheduler scheduler;

        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // 生成承载 web 应用程序的 Microsoft.AspNetCore.Hosting.IWebHost。Build是WebHostBuilder最终的目的，将返回一个构造的WebHost，最终生成宿主。
            var host = CreateHostBuilder(args).Build();

            // 定时任务
            // (PS:不推荐，建议用定时服务后台管理平台，通过Webapi方式调用)
            StartScheduler(host);

            // 运行 web 应用程序并阻止调用线程, 直到主机关闭。
            // 创建完 WebHost 之后，便调用它的 Run 方法，而 Run 方法会去调用 WebHost 的 StartAsync 方法
            // 将Initialize方法创建的Application管道传入以供处理消息
            // 执行HostedServiceExecutor.StartAsync方法
            host.Run();
        }

        /// <summary>
        /// 构建WebHost
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory()) //<--NOTE THIS
            .ConfigureWebHostDefaults(webBuilder =>
            {
                // 配置启动端口
                var config = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("hosting.json", optional: true)
                             .Build();
                webBuilder.UseConfiguration(config);

                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.AllowSynchronousIO = true;//启用同步 IO
                })
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                });
            });

        /// <summary>
        /// 执行定时任务
        /// (PS:不推荐，建议用定时服务后台管理平台，通过Webapi方式调用)
        /// </summary>
        /// <returns></returns>
        public static void StartScheduler(IHost host)
        {
            var schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();

            var job1 = JobBuilder.Create<AutoPushJob>()
                .WithIdentity("job1", "group1")
                .Build();

            var trigger1 = TriggerBuilder.Create()
                .WithIdentity("job1", "group1")
                .WithCronSchedule("0 0/1 * * * ? *")
                //.StartNow() // 不能马上执行，因为注入的信息还没拿到
                .Build();

            scheduler.ScheduleJob(job1, trigger1).Wait();
        }
    }
}
