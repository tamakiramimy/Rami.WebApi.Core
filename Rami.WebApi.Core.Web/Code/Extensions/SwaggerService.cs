using Microsoft.AspNetCore.Builder;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Rami.WebApi.Core.Web.Code.CustomApiVersion;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// Swagger服务
    /// </summary>
    public static class SwaggerServiceExtensions
    {
        /// <summary>
        /// API名称
        /// </summary>
        private const string ApiName = "公众号后台";

        /// <summary>
        /// 配置服务
        /// </summary>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            #region Swagger UI

            var basePath = ApplicationEnvironment.ApplicationBasePath;
            services.AddSwaggerGen(c =>
            {
                //遍历出全部的版本，做文档信息展示
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    var apiInfo = new OpenApiInfo
                    {
                        // {ApiName} 定义成全局变量，方便修改
                        Version = version,
                        Title = $"{ApiName} 接口文档——Netcore 3.0",
                        Description = $"{ApiName} HTTP API " + version,
                    };
                    c.SwaggerDoc(version, apiInfo);
                    c.OrderActionsBy(o => o.RelativePath);
                });

                // 实体注释
                var domainComments = Path.Combine(basePath, "Rami.WebApi.Core.Domain.xml");
                c.IncludeXmlComments(domainComments);

                // API注释
                var apiComments = Path.Combine(basePath, "Rami.WebApi.Core.Web.xml");//这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(apiComments, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                //// 企业微信SDK
                //var wxComments = Path.Combine(basePath, "Rami.Wechat.Core.xml");
                //c.IncludeXmlComments(wxComments);

                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                // Token绑定到ConfigureServices
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });

            #endregion

            return services;
        }

        /// <summary>
        /// 使用服务
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UserSwaggerService(this IApplicationBuilder app)
        {
            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //根据版本名称倒序 遍历展示
                typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });
                // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：解决方案名.index.html
                c.IndexStream = () => typeof(SwaggerServiceExtensions).GetTypeInfo().Assembly.GetManifestResourceStream("Rami.WebApi.Core.Web.swagger.html");//这里是配合MiniProfiler进行性能监控的，《文章：完美基于AOP的接口性能分析》，如果你不需要，可以暂时先注释掉，不影响大局。
                c.RoutePrefix = "swagger"; //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
            });

            return app;
        }
    }
}
