using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// Log4日志
    /// </summary>
    public static class Log4Service
    {
        /// <summary>
        /// AddLog4Service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="conf"></param>
        /// <param name="env"></param>
        public static IServiceCollection AddLog4Service(this IServiceCollection services, IConfiguration conf, IWebHostEnvironment env)
        {
            // log4net配置
            var log4Repo = LogManager.CreateRepository(conf["Logging:Log4Net:Name"]);
            //指定配置文件，如果这里你遇到问题，应该是使用了InProcess模式，请查看Blog.Core.csproj,并删之
            var log4Config = Path.Combine(env.ContentRootPath, "Log4net.config");
            XmlConfigurator.Configure(log4Repo, new FileInfo(log4Config));
            return services;
        }
    }
}
