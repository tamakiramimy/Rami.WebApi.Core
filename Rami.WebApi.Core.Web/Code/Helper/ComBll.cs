using Rami.WebApi.Core.Domain;
using Rami.WebApi.Core.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// 公用业务
    /// </summary>
    public class ComBll
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IRepository works;

        /// <summary>
        /// 用户
        /// </summary>
        private readonly CurrentUser currentUser;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="works"></param>
        /// <param name="currentUser"></param>
        public ComBll(IRepository works, CurrentUser currentUser)
        {
            this.works = works;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// 通用保存
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task CommSave<T>(T item) where T : BaseDbShow, new()
        {
            var dtNow = DateTime.Now;
            var userName = currentUser.UserName;
            if (!item.CreateTime.HasValue)
            {
                item.CreateTime = dtNow;
                item.Creater = userName;
            }

            item.UpdateTime = dtNow;
            item.Updater = userName;
            await works.SaveAsync(item);
        }

        /// <summary>
        /// 通用更新方法
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> CommUpd<T>(T item) where T : BaseDbShow, new()
        {
            var dtNow = DateTime.Now;
            item.IsDel = 1 - item.IsDel;
            item.UpdateTime = dtNow;
            item.Updater = currentUser.UserName;
            var isSucc = await works.UpdateAsync(item, new List<string> { "IsDel", "UpdateTime", "Updater" });
            return isSucc;
        }
    }
}
