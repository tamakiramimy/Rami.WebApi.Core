using Autofac;
using Autofac.Extras.DynamicProxy;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// AutofacService
    /// </summary>
    public static class AutofacServiceExtensions
    {
        /// <summary>
        /// AutofacConfigure
        /// </summary>
        /// <param name="builder"></param>
        public static ContainerBuilder AutofacConfigure(this ContainerBuilder builder)
        {
            // 注册服务
            builder.RegisterServices();
            return builder;
        }

        /// <summary>
        /// RegisterServices
        /// </summary>
        /// <param name="builder"></param>
        internal static void RegisterServices(this ContainerBuilder builder)
        {
            var basePath = ApplicationEnvironment.ApplicationBasePath;

            //AOP拦截必须是接口或者虚方法
            //注册要通过反射创建的组件
            builder.RegisterType<MemCacheAOP>();//可以直接替换其他拦截器
            builder.RegisterType<RedisCacheAOP>();//可以直接替换其他拦截器
            builder.RegisterType<LogAOP>();//这样可以注入第二个

            #region 带有接口层的服务注入

            try
            {
                #region 注入其他Dll

                var dllFrmework = Path.Combine(basePath, "Rami.WebApi.Core.Framework.dll");
                var assemblyFrmework = Assembly.LoadFrom(dllFrmework);
                builder.RegisterAssemblyTypes(assemblyFrmework).AsImplementedInterfaces();

                // IRepository--多数据库切换开启  IocManager.Instance.ResolveNamed<IRepository>("WechatHr");
                //builder.RegisterType<RepositorySugar>().WithParameter("connPath", "AppSettings:WechatHr").Named<IRepository>("WechatHr").InstancePerLifetimeScope();
                //builder.RegisterType<RepositorySugar>().WithParameter("connPath", "AppSettings:WechatHrOrg").Named<IRepository>("WechatHrOrg").InstancePerLifetimeScope();

                #endregion

                #region Service.dll 注入，有对应接口

                // AOP 开关，如果想要打开指定的功能，只需要在 appsettigns.json 对应对应 true 就行。
                var cacheType = new List<Type>();
                if (ConfigHelper.IsMemoryAopEnabled)
                {
                    cacheType.Add(typeof(MemCacheAOP));
                }

                if (ConfigHelper.IsRedisAopEnabled)
                {
                    cacheType.Add(typeof(RedisCacheAOP));
                }

                if (ConfigHelper.IsLogAopEnabled)
                {
                    cacheType.Add(typeof(LogAOP));
                }

                // 引用Autofac.Extras.DynamicProxy;
                // 如果你想注入两个，就这么写  InterceptedBy(typeof(BlogCacheAOP), typeof(BlogLogAOP));
                // 如果想使用Redis缓存，请必须开启 redis 服务，端口号我的是6319，如果不一样还是无效，否则请使用memory缓存 BlogCacheAOP
                // 允许将拦截器服务的列表分配给注册。 
                // 直接采用加载文件的方法  ※※★※※ 如果你是第一次下载项目，请先F6编译，然后再F5执行，※※★※※
                var dllService = Path.Combine(basePath, "Rami.WebApi.Core.Service.dll");
                var assemblysService = Assembly.LoadFrom(dllService);
                builder.RegisterAssemblyTypes(assemblysService)
                          .AsImplementedInterfaces()
                          .InstancePerLifetimeScope()
                          .EnableInterfaceInterceptors()
                          .InterceptedBy(cacheType.ToArray());

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion
        }
    }

    /// <summary>
    /// 静态Autofac IocManager
    /// </summary>
    public class AutofacServiceContainer : Autofac.Module
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors();

            // 允许属性注入 允许在Controller类上使用拦截器
            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired()
                .EnableClassInterceptors();

            // 注册服务
            builder.RegisterServices();

            // 注册完服务返回IContainer
            builder.RegisterBuildCallback(container =>
            {
                StaticAutofacIocManger.Configure(container);
                IocManager.Configure(container);
            });
        }
    }
}
