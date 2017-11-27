using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgileFramework.Net
{
    /// <summary>
    /// 基于.NET基础类的缓存池，需要先注入
    /// </summary>
    public class AgileHttpCache
    {
        /// <summary>
        /// 基于.NET的缓存器
        /// </summary>
        protected static IMemoryCache cache;

        public AgileHttpCache(IMemoryCache _cache)
        {
            cache = _cache;
        }

        /// <summary>
        /// 插入缓存项
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <returns>是否插入</returns>
        public bool Add(string key, object obj)
        {
            if (obj != null && !string.IsNullOrEmpty(key))
            {
                cache.Set(key, obj);
            }
            return ContainsKey(key);
        }

        /// <summary>
        /// 插入指定超时时间的缓存项
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <param name="TimeOut">超时时间</param>
        /// <returns>是否插入</returns>
        public bool Add(string key, object obj, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (obj != null && !string.IsNullOrEmpty(key))
            {
                cache.Set(key, obj,
                     new MemoryCacheEntryOptions()
                     .SetSlidingExpiration(expiresSliding)
                     .SetAbsoluteExpiration(expiressAbsoulte)
                     );
            }
            return ContainsKey(key);
        }

        public bool Add(string key, object obj, TimeSpan expiresIn, bool isSliding = false)
        {
            if (obj != null && !string.IsNullOrEmpty(key))
            {
                if (isSliding)
                {
                    cache.Set(key, obj,
                        new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(expiresIn)
                        );
                }
                else
                {
                    cache.Set(key, obj,
                    new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(expiresIn)
                    );
                }
            }

            return ContainsKey(key);
        }

        /// <summary>
        /// 是否包含键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            object obj;

            return cache.TryGetValue(key, out obj);
        }

        /// <summary>
        /// 删除指定项
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                cache.Remove(key);
            }
            return !ContainsKey(key);
        }

        /// <summary>
        /// 删除所有项
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll(IEnumerable<string> keys)
        {
            lock (cache)
            {
                keys.ToList().ForEach(item => cache.Remove(item));
            }
            return true;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return cache.Get(key);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return (T)cache.Get(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Set(string key, object obj)
        {
            if (ContainsKey(key))
            {
                Delete(key);
            }
            return Add(key, obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="TimeOut"></param>
        /// <returns></returns>
        public bool Set(string key, object obj, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (ContainsKey(key))
            {
                Delete(key);
            }
            return Add(key, obj, expiresSliding, expiressAbsoulte);
        }

        public bool Set(string key, object obj, TimeSpan expiresIn, bool isSliding = false)
        {
            if (ContainsKey(key))
            {
                Delete(key);
            }
            return Add(key, obj, expiresIn, isSliding);
        }

        public void Dispose()
        {
            if (cache != null)
            {
                cache.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
