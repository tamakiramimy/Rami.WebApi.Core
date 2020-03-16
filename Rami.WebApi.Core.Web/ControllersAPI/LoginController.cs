using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using Rami.WebApi.Core.Service;
using Rami.WebApi.Core.Web.Code;
using Microsoft.AspNetCore.Mvc;

namespace Rami.WebApi.Core.Web.ControllersAPI
{
    /// <summary>
    /// 登陆
    /// </summary>
    public class LoginController : BaseController
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogHelper<LoginController> logHelper;

        /// <summary>
        /// 仓储
        /// </summary>
        private readonly IRepository repo;

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly ICacheHelper cache;

        /// <summary>
        /// 当前用户
        /// </summary>
        private readonly CurrentUser currentUser;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logHelper"></param>
        /// <param name="repo"></param>
        /// <param name="cache"></param>
        /// <param name="currentUser"></param>
        public LoginController(ILogHelper<LoginController> logHelper, CurrentUser currentUser, IRepository repo, ICacheHelper cache)
        {
            this.logHelper = logHelper;
            this.currentUser = currentUser;
            this.repo = repo;
            this.cache = cache;
        }

        #region Jwt+Cookies

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        [HttpGet("GetJwtByLogin")]
        public async Task<Result<LoginParam>> GetJwtByLogin([FromQuery]string userName, string userPwd)
        {
            var loginRes = await currentUser.Login(new LoginModel { UserName = userName, Password = userPwd });
            return loginRes;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task<Result> Logout()
        {
            try
            {
                await AuthHelper.IdentifyLogout();
                return new Result { IsSucc = true };
            }
            catch (Exception ex)
            {
                logHelper.Error($"Logout:退出登录失败:{ex.Message}", ex);
            }

            return new Result { };
        }

        /// <summary>
        /// 刷新Jwt
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("GetJwtByRefresh")]
        public async Task<Result<LoginParam>> GetJwtByRefresh([FromQuery]string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new Result<LoginParam> { Message = "token无效，请重新登录！" };
            }

            var tokenModel = AuthHelper.DecryptJwtToken(token);
            if (tokenModel != null && tokenModel.Uid > 0)
            {
                // 判断用户是否存在
                var user = cache.GetUserInfoCache().FirstOrDefault(x => x.UserName == tokenModel.UserName);
                if (user != null)
                {
                    var refreshToken = await AuthHelper.GetJwtToken(tokenModel);
                    return new Result<LoginParam> { IsSucc = true, Data = refreshToken, Message = "刷新Jwt成功！" };
                }
            }

            return new Result<LoginParam> { Message = "刷新Jwt失败" };
        }

        /// <summary>
        /// 根据token获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("GetUserByJwt")]
        public async Task<Result<UserInfoShow>> GetUserByJwt([FromQuery]string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new Result<UserInfoShow> { Message = "token无效，请重新登录！" };
            }

            var tokenModel = AuthHelper.DecryptJwtToken(token);
            if (tokenModel != null && tokenModel.Uid > 0)
            {
                // 判断是否当前用户
                if (tokenModel.UserName == currentUser.UserName)
                {
                    // 判断用户是否存在
                    var user = await repo.FirstOrDefaultAsync<UserInfoShow>(x => x.UserName == tokenModel.UserName);
                    if (user != null)
                    {
                        return new Result<UserInfoShow> { IsSucc = true, Data = user };
                    }
                }
            }

            return new Result<UserInfoShow> { Data = null };
        }

        #endregion
    }
}