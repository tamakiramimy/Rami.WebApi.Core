using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    #region Cookies Scheme实现API授权认证

    /// <summary>
    /// API授权(Cookies)
    /// 配合ApiAuthorizeHandlerSimple使用，可以让所有API都执行OnAuthorization验证
    /// </summary>
    public class ApiAuthorize : Attribute, IAuthorizeData, IAuthorizationFilter
    {
        /// <summary>
        /// Scheme认证（多个可以用，隔开）
        /// </summary>
        public string AuthenticationSchemes { get; set; } = AuthHelper.JwtAuthScheme;

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
        /// 当前用户
        /// </summary>
        private CurrentUser currentUser => StaticServiceProvider.Current.GetRequiredService<CurrentUser>();

        /// <summary>
        /// 授权判断
        /// 配合ApiAuthorizeHandlerSimple使用，可以让所有API都执行OnAuthorization验证
        /// PS:只有不需要授权的方法才会进来，需要授权的都会被拦截跳转到StartUp注册的LoginPath，授权成功后才会进来这里。。。
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
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

            context.HttpContext.Response.StatusCode = 401;
            context.Result = new JsonResult(ComHelper.JsonSerialize(new Result { Message = "授权失败！" }));
            return;
        }
    }

    /// <summary>
    /// 配合Cookies 认证使用
    /// 允许所有API进入ApiAuthorize的OnAuthorization，不拦截没授权的请求
    /// </summary>
    public class ApiAuthorizeHandlerSimple : AuthorizationHandler<ApiAuthorizeHandlerSimple>, IAuthorizationRequirement
    {
        /// <summary>
        /// 允许所有API进入ApiAuthorize的OnAuthorization，不拦截没授权的请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAuthorizeHandlerSimple requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    #endregion

    #region Jwt Scheme 实现API授权认证

    /// <summary>
    /// API授权请求资源
    /// </summary>
    public class ApiAuthorizeHandlerReq : IAuthorizationRequirement
    {
        /// <summary>
        /// 角色
        /// </summary>
        public EnumSysRoles[] Roles { get; set; }

        /// <summary>
        /// Jwt发行人
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Jwt订阅人
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Jwt过期时间
        /// </summary>
        public TimeSpan Expiration { get; set; }

        /// <summary>
        /// Jwt签名验证
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }

    /// <summary>
    /// API授权处理
    /// </summary>
    public class ApiAuthorizeHandler : AuthorizationHandler<ApiAuthorizeHandlerReq>
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
        public ApiAuthorizeHandler(IAuthenticationSchemeProvider schemes, IRepository repository, IHttpContextAccessor httpContextAccessor, ILogHelper<ApiAuthorizeHandler> logHelper)
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
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAuthorizeHandlerReq requirement)
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
                    #region 微软Jwt中间件已经实现了Authorzation的验证

                    //// jwt授权信息在Header
                    //// 检测是否包含'Authorization'请求头
                    //var httpHeader = httpContext.Request.Headers;
                    //if (httpHeader.ContainsKey("Authorization"))
                    //{
                    //    var tokenHeader = httpHeader["Authorization"].ToString().Replace("Bearer ", "");
                    //    try
                    //    {
                    //        if (tokenHeader.Length >= 128)
                    //        {
                    //            // 解析jwt，获取用户信息，进行jwt授权登录
                    //            var tokenInfo = JwtHelper.DecryptJwtToken(tokenHeader);
                    //            var claims = new List<Claim>
                    //            {
                    //                new Claim(JwtRegisteredClaimNames.Jti, tokenInfo.Uid.ToString()),
                    //                new Claim(ClaimTypes.Name,tokenInfo.UserName),
                    //            };

                    //            claims.AddRange(tokenInfo.Role.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));

                    //            // 保存用户信息到HttpContext.User
                    //            await JwtHelper.IdentifyLogin(claims);

                    //            // todo:根据角色判断是否有权限调用
                    //            AuthDeal(context, requirement, httpContext, true);
                    //            return;
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        LogHelper.Error($"ApiAuthorizeHandler:解析Jwt失败:{ex.Message}", ex);
                    //    }
                    //}

                    #endregion

                    var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                    if (result?.Principal != null)
                    {
                        httpContext.User = result.Principal;
                        //判断过期时间（这里仅仅是最坏验证原则，你可以不要这个if else的判断，因为我们使用的官方验证，Token过期后上边的result?.Principal 就为 null 了，进不到这里了，因此这里其实可以不用验证过期时间，只是做最后严谨判断）
                        if ((httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null && DateTime.Parse(httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) >= DateTime.Now)
                        {
                            AuthDeal(context, requirement, httpContext, true);
                            return;
                        }

                        // todo:根据角色判断是否有权限调用
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
        private static void AuthDeal(AuthorizationHandlerContext context, ApiAuthorizeHandlerReq requirement, HttpContext httpContext, bool isAuth)
        {
            // api路由
            if (isAuth)
            {
                // 授权成功
                context.Succeed(requirement);
                return;
            }
            else
            {
                // 授权失败，返回401
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                //// 返回Json
                //var jsonRes = ComHelper.JsonSerialize(new Result { Code = ((int)HttpStatusCode.Unauthorized).ToString(), Message = HttpStatusCode.Unauthorized.ToString() });
                //httpContext.Response.ContentType = "application/json;charset=utf-8";
                //httpContext.Response.WriteAsync(jsonRes);

                //context.Succeed(requirement);// 如果启用了Cookies认证，会直接执行API
                context.Fail();// 如果启用了Cookies认证，会直接跳到Cookie认证的登陆界面
                return;
            }
        }
    }

    #endregion
}
