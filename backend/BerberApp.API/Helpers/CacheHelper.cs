using Microsoft.Extensions.Caching.Memory;
using System;

namespace BerberApp.API.Helpers
{
    public class CacheHelper
    {
        private readonly IMemoryCache _memoryCache;

        public CacheHelper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Cache'de key ile kayıtlı veri varsa döner, yoksa factory fonksiyon ile oluşturup cache'e ekler.
        /// </summary>
        /// <typeparam name="T">Cache'deki veri tipi</typeparam>
        /// <param name="key">Cache anahtarı</param>
        /// <param name="factory">Eğer cache yoksa veriyi oluşturacak fonksiyon</param>
        /// <param name="absoluteExpirationMinutes">Cache süresi (dakika), default 60 dakika</param>
        /// <returns></returns>
        public T? GetOrCreate<T>(string key, Func<T?> factory, int absoluteExpirationMinutes = 60)
        {
            if (_memoryCache.TryGetValue(key, out T? cacheEntry))
            {
                return cacheEntry;
            }

            cacheEntry = factory();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(absoluteExpirationMinutes));

            if (cacheEntry != null)
            {
                _memoryCache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }

        /// <summary>
        /// Cache'den belirli bir key ile kayıtlı veriyi siler.
        /// </summary>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <summary>
        /// Cache'de belirli bir key'in olup olmadığını kontrol eder.
        /// </summary>
        public bool Exists(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }
    }
}
