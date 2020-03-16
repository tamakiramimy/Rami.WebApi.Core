using Rami.WebApi.Core.Framework;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rami.WebApi.Core.Web.Code
{
    /// <summary>
    /// MemoryCaching缓存AOP
    /// 使用方法：配置Appsetting启用MemoryCaching缓存，并且在需要缓存的方法添加CachingAttribute属性，设置缓存时间（分钟）
    /// </summary>
    public class MemCacheAOP : CacheAOPbase
    {
        /// <summary>
        /// 通过注入的方式，把缓存操作接口通过构造函数注入
        /// </summary>
        private readonly ICaching _cache;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cache"></param>
        public MemCacheAOP(ICaching cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Intercept方法是拦截的关键所在，也是IInterceptor接口中的唯一定义
        /// </summary>
        /// <param name="invocation"></param>
        public override void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法的特性验证
            //如果需要验证
            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) is CachingAttribute qCachingAttribute)
            {
                //获取自定义缓存键
                var cacheKey = CustomCacheKey(invocation);
                //根据key获取相应的缓存值
                var cacheValue = _cache.Get(cacheKey);
                if (cacheValue != null)
                {
                    //将当前获取到的缓存值，赋值给当前执行方法
                    invocation.ReturnValue = cacheValue;
                    return;
                }

                //去执行当前的方法
                invocation.Proceed();
                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    _cache.Set(cacheKey, invocation.ReturnValue);
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }
    }
}
