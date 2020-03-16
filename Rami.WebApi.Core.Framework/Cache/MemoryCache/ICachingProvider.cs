namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 简单的缓存接口，只有查询和添加，以后会进行扩展
    /// </summary>
    public interface ICaching
    {
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        object Get(string cacheKey);

        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="cacheSec"></param>
        void Set(string cacheKey, object cacheValue, int cacheSec = 60);
    }
}
