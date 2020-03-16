using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Threading.Tasks;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Rami.WebApi.Core.Web.Code;
using Autofac;
using AutoMapper;
using log4net;
using log4net.Config;
using log4net.Repository;
using Rami.Wechat.Core.Comm;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UEditorNetCore;
using Quartz;
using Quartz.Impl;

namespace Rami.WebApi.Core.Web
{
    /// <summary>
    /// 程序启动
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 配置注入
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 环境变量
        /// </summary>
        public IWebHostEnvironment Env { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        /// <summary>
        /// 把服务添加到容器
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // log4日志
            services.AddLog4Service(Configuration, Env);

            // MemoryCaching缓存注入
            services.AddMemoryCacheService();

            // Redis缓存注入
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();

            // log日志注入
            services.AddTransient(typeof(ILogHelper<>), typeof(LogHelper<>));

            // 静态Appsetting
            services.AddSingleton(new Appsettings(Env.ContentRootPath));

            // 初始化DB
            services.AddScoped<SugarDbSeed>();

            // AutoMapper
            services.AddAutoMapper(typeof(Startup));

            // 跨域请求配置
            services.AddCorsService();

            // MiniProfiler-Swagger API性能监控
            services.AddMiniProfilerService();

            // 配置Swagger
            services.AddSwagger();

            // 添加Cont和View
            services.AddControllersWithViewsService();

            // Httpcontext 注入
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 全局用户
            services.AddScoped<IUser, AspNetUser>();

            // SignalR 通讯
            services.AddSignalR().AddNewtonsoftJsonProtocol();

            if (ConfigHelper.IsOidcAuth)
            {
                // Id4 Oidc认证
                services.AddId4OidcService();
            }
            else
            {
                // cookies认证(mvc)
                services.AddCookiesAuthService();

                // Jwt认证(api)
                services.AddJwtAuthService();
            }

            // 授权策略
            services.AddAuthorizationService();

            //// 正式环境强制http跳转https
            //if (!Env.IsDevelopment())
            //{
            //    services.AddHttpsRedirection(options =>
            //    {
            //        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            //        options.HttpsPort = 443;
            //    });
            //}

            // 使用UEditor
            services.AddUEditorService(configFile: "ueconfig.json");

            // 其他需要注入的非接口类
            //services.AddScoped<ADHelper>();
            services.AddScoped<LdapHelper>();
            services.AddScoped<ComBll>();
            services.AddScoped<CurrentUser>();
            services.AddSingleton(typeof(IIocManager), IocManager.Instance);

            // 自动回复
            services.AddScoped<WxAutoComResponse>();
            services.AddScoped<WxAutoConvertHelper>();
            services.AddScoped<WxAutoKeywordHelper>();
            services.AddScoped<WxAutoPushHelper>();
            services.AddScoped<WxAutoResponseHelper>();

            // 微信通知
            services.AddScoped<WxDbLogHelper>();
            services.AddScoped<WxEventClickHelper>();
            services.AddScoped<WxEventHandler>();
            services.AddScoped<WxEventScanHelper>();
            services.AddScoped<WxKfTransferHelper>();
            services.AddScoped<WxTextHandler>();

            // 定时服务
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddScoped<AutoPushSvc>();
        }

        /// <summary>
        /// 配置Http管道
        ///  This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // API请求响应日志
            if (ConfigHelper.IsApiReqResMidLog)
            {
                app.UseReuestResponseLog();
            }

            // 环境变量
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Swagger
            app.UserSwaggerService();

            // MiniProfiler
            app.UseMiniProfiler();

            // 跨域第二种方法，使用策略，详细策略信息在ConfigureService中
            app.UseCors("LimitRequests"); //将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求。

            // 静态文件
            app.UseStaticFiles();

            // 使用cookie
            app.UseCookiePolicy();

            // 把错误码返回前台，比如是404
            app.UseStatusCodePages();

            // 路由
            app.UseRouting();

            //// 正式环境强制使用https
            //if (!Env.IsDevelopment())
            //{
            //    app.UseHsts();
            //    app.UseHttpsRedirection();
            //}

            //// 此授权认证方法已经放弃，请使用下边的官方验证方法。但是如果你还想传User的全局变量，还是可以继续使用中间件，第二种写法//app.UseMiddleware<JwtTokenAuth>(); 
            //app.UseJwtTokenAuth();

            // 如果你想使用官方认证，必须在上边ConfigureService 中，配置JWT的认证服务 (.AddAuthentication 和 .AddJwtBearer 二者缺一不可)
            app.UseAuthentication();

            // 授权
            app.UseAuthorization();

            // 静态HttpContext
            app.UseStaticHttpContext();

            // 静态ServiceProvider
            app.UseStaticServiceProvider();

            // 把服务注入到微信类库
            app.UseWxServiceProvider();

            // 路由配置
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Admin}/{action=Index}/{id?}");

                endpoints.MapHub<ChatHub>("/api2/chatHub");

                // 默认首页
                endpoints.MapGet("/", async ctx =>
                {
                    var startUlr = ConfigHelper.StartUrl;
                    ctx.Response.Redirect(startUlr);
                    await Task.CompletedTask;
                });
            });

            //// 默认首页
            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("backstage.html");
            //app.UseDefaultFiles(defaultFilesOptions);

            //// 默认首页
            //app.Run(ctx =>
            //{
            //    ctx.Response.Redirect("/backstage.html"); //可以支持虚拟路径或者index.html这类起始页.
            //    return Task.FromResult(0);
            //});
        }

        /// <summary>
        /// IOC容器
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.AutofacConfigure();
            builder.RegisterModule<AutofacServiceContainer>();
        }
    }
}