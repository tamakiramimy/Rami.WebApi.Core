using Rami.WebApi.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rami.WebApi.Core.Framework;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Rami.WebApi.Core.Web.Code
{
    #region Cookies Scheme实现Mvc授权认证

    /// <summary>
    /// MVC授权认证
    /// 配合MvcAuthrizeHandlerSimple使用，可以让所有Action都执行OnAuthorization验证
    /// </summary>
    public class MvcAuthorize : Attribute, IAuthorizationFilter, IAuthorizeData
    {
        /// <summary>
        /// Scheme认证（多个可以用，隔开）
        /// </summary>
        public string AuthenticationSchemes { get; set; } = AuthHelper.CookieAuthScheme;

        /// <summary>
        /// 策略认证（多个可以用，隔开）
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// 角色认证（多个可以用，隔开）
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// 角色认证（重写）
        /// </summary>
        public EnumSysRoles[] RoleList { get; set; }

        /// <summary>
        /// 授权失败跳转授权界面(无效，因为netcore未授权默认会跳转到StartUp配置的LoginPath)
        /// </summary>
        public string RedirUrl { get; set; } = "/Admin/Login";

        /// <summary>
        /// 当前用户
        /// </summary>
        private CurrentUser currentUser = StaticServiceProvider.Current.GetRequiredService<CurrentUser>();

        /// <summary>
        /// 授权判断
        /// 配合MvcAuthrizeHandlerSimple使用，可以让所有Action都执行OnAuthorization验证
        /// PS:只有不需要授权的方法才会进来，需要授权的都会被拦截跳转到StartUp注册的LoginPath，授权成功后才会进来这里。。。
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnAuthorization(AuthorizationFilterContext context)
        {
            // 是否跳过授权
            var isSkipAuthorize = context.ActionDescriptor.EndpointMetadata.Where(a => a is AllowAnonymousAttribute).Any();
            if (isSkipAuthorize)
            {
                return;
            }

            // 判断用户是否已经授权
            var isAuth = context.HttpContext.AuthenticateAsync(AuthenticationSchemes).Result.Succeeded;
            // 判断用户角色
            var checkRoles = RoleList != null && RoleList.Length > 0 ? currentUser.IsHasRole(RoleList) : true;
            if (isAuth && checkRoles)
            {
                return;
            }

            // 无授权，跳转授权界面
            var url = $"{RedirUrl}?returnUrl={context.HttpContext.Request.Path}";
            context.Result = new RedirectResult(url);
            return;
        }
    }

    /// <summary>
    /// 配合Cookies 认证使用
    /// 允许所有Mvc进入MvcAuthorize的OnAuthorization，不拦截没授权的请求
    /// </summary>
    public class MvcAuthorizeHandlerSimple : AuthorizationHandler<MvcAuthorizeHandlerSimple>, IAuthorizationRequirement
    {
        /// <summary>
        /// 允许所有Mvc进入MvcAuthorize的OnAuthorization
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MvcAuthorizeHandlerSimple requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    #endregion

    #region Jwt Scheme 实现Mvc授权认证

    /// <summary>
    /// Mvc 角色路由配置
    /// </summary>
    public class MvcRoleUrlPermissions
    {
        /// <summary>
        /// 用户或角色或其他凭据名称
        /// </summary>
        public virtual string Role { get; set; }

        /// <summary>
        /// 请求Url
        /// </summary>
        public virtual string Url { get; set; }
    }

    /// <summary>
    /// Mvc授权请求资源
    /// </summary>
    public class MvcAuthorizeHandlerReq : IAuthorizationRequirement
    {
        /// <summary>
        /// 角色
        /// </summary>
        public EnumSysRoles[] Roles { get; set; }

        /// <summary>
        /// 用户权限集合，一个订单包含了很多详情，
        /// 同理，一个网站的认证发行中，也有很多权限详情(这里是Role和URL的关系)
        /// </summary>
        public List<MvcRoleUrlPermissions> Permissions { get; set; }

        /// <summary>
        /// 认证授权类型
        /// </summary>
        public string ClaimType { internal get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        public string LoginPath { get; set; } = "/Admin/Login";

        /// <summary>
        /// 无权限action
        /// </summary>
        public string DeniedAction { get; set; }
    }

    /// <summary>
    /// Mvc授权处理
    /// 单独Jwt认证使用（缺点：Mvc的Action返回都会执行一次后，才会触发跳转）
    /// </summary>
    public class MvcAuthorizeHandler : AuthorizationHandler<MvcAuthorizeHandlerReq>
    {
        /// <summary>
        /// IAuthenticationSchemeProvider
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }

        /// <summary>
        /// IRepository
        /// </summary>
        public IRepository Repository { get; set; }

        /// <summary>
        /// IHttpContextAccessor
        /// </summary>
        public IHttpContextAccessor HttpContextAccessor;

        /// <summary>
        /// ILogHelper
        /// </summary>
        public ILogHelper<ApiAuthorizeHandler> LogHelper { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="schemes"></param>
        /// <param name="repository"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logHelper"></param>
        public MvcAuthorizeHandler(IAuthenticationSchemeProvider schemes, IRepository repository, IHttpContextAccessor httpContextAccessor, ILogHelper<ApiAuthorizeHandler> logHelper)
        {
            Schemes = schemes;
            Repository = repository;
            HttpContextAccessor = httpContextAccessor;
            LogHelper = logHelper;
        }

        /// <summary>
        /// 判断是否授权
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MvcAuthorizeHandlerReq requirement)
        {
            #region 说明

            /*
            * .netcore3.0 启用EndpointRouting后，权限filter不再添加到ActionDescriptor ，而将权限直接作为中间件运行，
            * 同时所有filter都会添加到endpoint.Metadata。因此，文中的
            * context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext不再成立。
            * 
            * 解决方案有两个：
            * 
            * 首先必须在 controller 上进行配置 Authorize ，可以策略授权，也可以角色等基本授权
            * 
            * 1、开启公约， startup 中的全局授权过滤公约：o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
            * 
            * 2、不开启公约，使用 IHttpContextAccessor ，也能实现效果，但是不能自定义返回格式，详细看下边配置；
            */

            #endregion

            //从AuthorizationHandlerContext转成HttpContext，以便取出表求信息(netcore2.2之前有效，3.0需要开启全局过滤公约才有这个上下文)
            var filterContext = (context.Resource as AuthorizationFilterContext);
            var httpContext = (context.Resource as AuthorizationFilterContext)?.HttpContext;
            if (httpContext == null)
            {
                httpContext = HttpContextAccessor.HttpContext;
            }

            //请求Url
            if (httpContext != null)
            {
                var user = httpContext.User;

                //判断请求是否停止
                var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
                {
                    if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                    {
                        AuthDeal(context, requirement, httpContext, false);
                        return;
                    }
                }

                //判断请求是否拥有凭据，即有没有登录
                var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                    if (result?.Principal != null)
                    {
                        httpContext.User = result.Principal;

                        //判断过期时间（这里仅仅是最坏验证原则，你可以不要这个if else的判断，因为我们使用的官方验证，Token过期后上边的result?.Principal 就为 null 了，进不到这里了，因此这里其实可以不用验证过期时间，只是做最后严谨判断）
                        if ((httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null && DateTime.Parse(httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) >= DateTime.Now)
                        {
                            // todo:根据用户判断是否 有对应角色和路由的权限
                            //var questUrl = httpContext.Request.Path.Value.ToLower();
                            if (requirement.Roles != null && requirement.Roles.Length > 0)
                            {
                                var strReqRoles = string.Join(",", requirement.Roles.Select(x => ((int)x).ToString()));
                                var isUserHadRoles = httpContext.User.IsInRole(strReqRoles);
                                if (!isUserHadRoles)
                                {
                                    AuthDeal(context, requirement, httpContext, false);
                                    return;
                                }
                            }

                            if (true)
                            {
                                AuthDeal(context, requirement, httpContext, true);
                                return;
                            }
                        }
                    }
                }
            }

            AuthDeal(context, requirement, httpContext, false);
            return;
        }

        /// <summary>
        /// 处理授权结果
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <param name="httpContext"></param>
        /// <param name="isAuth"></param>
        private static void AuthDeal(AuthorizationHandlerContext context, MvcAuthorizeHandlerReq requirement, HttpContext httpContext, bool isAuth)
        {
            // mvc路由
            if (isAuth)
            {
                // 授权成功
                context.Succeed(requirement);
                return;
            }
            else
            {
                var redirUrl = $"{requirement.LoginPath}?returnUrl={httpContext.Request.Path.Value}";
                // 授权失败，跳转首页
                httpContext.Response.Redirect(redirUrl);
                httpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;
                //context.Fail();// 如果启用了Cookie认证，会跳转到授权首页；如果只启用了Jwt认证，会返回401
                context.Succeed(requirement);// 如果只启用了Jwt认证，还是会执行View的操作，只是最后会执行上面定义的跳转
                return;
            }
        }
    }

    #endregion
}
