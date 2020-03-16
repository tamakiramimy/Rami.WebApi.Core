using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface IRedisCacheManager
    {
        /// <summary>
        /// 获取 Reids 缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetValue(string key);

        /// <summary>
        /// 获取值，并序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity Get<TEntity>(string key);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        void Set(string key, object value, TimeSpan cacheTime);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsExist(string key);

        /// <summary>
        /// 移除某一个缓存值
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 全部清除
        /// </summary>
        void Clear();
    }
}
