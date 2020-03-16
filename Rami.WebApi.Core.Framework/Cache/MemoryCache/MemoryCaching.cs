using Microsoft.Extensions.Caching.Memory;
using System;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 实例化缓存接口ICaching
    /// </summary>
    public class MemoryCaching : ICaching
    {
        /// <summary>
        /// 引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 还是通过构造函数的方法，获取
        /// </summary>
        /// <param name="cache"></param>
        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }

        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="cacheSec"></param>
        public void Set(string cacheKey, object cacheValue, int cacheSec = 60)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(cacheSec));
        }
    }
}
