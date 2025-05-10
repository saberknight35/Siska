using Siska.Admin.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Siska.Admin.Cache
{
    public class DistributedCacheService : ICacheService
    {
        private ILogger<DistributedCacheService> _logger;
        private readonly IDistributedCache _cache;

        public DistributedCacheService(IDistributedCache cache
            , ILogger<DistributedCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T Get<T>(string key) => Get(key) is { } data ? Deserialize<T>(data) : default;

        private byte[] Get(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            try
            {
                return _cache.Get(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                //return null;
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken token = default) =>
            await GetAsync(key, token) is { } data
                ? Deserialize<T>(data)
                : default;

        private async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            try
            {
                return await _cache.GetAsync(key, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                //return null;
                throw;
            }
        }

        public void Refresh(string key)
        {
            try
            {
                _cache.Refresh(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            try
            {
                await _cache.RefreshAsync(key, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public void Remove(string key)
        {
            try
            {
                _cache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            try
            {
                await _cache.RemoveAsync(key, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null) =>
            Set(key, Serialize(value), slidingExpiration);

        private void Set(string key, byte[] value, TimeSpan? slidingExpiration = null)
        {
            try
            {
                _cache.Set(key, value, GetOptions(slidingExpiration));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default) =>
            SetAsync(key, Serialize(value), slidingExpiration, cancellationToken);

        private async Task SetAsync(string key, byte[] value, TimeSpan? slidingExpiration = null, CancellationToken token = default)
        {
            try
            {
                await _cache.SetAsync(key, value, GetOptions(slidingExpiration), token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private byte[] Serialize<T>(T item)
        {
            return Encoding.Default.GetBytes(NewtonSoftSerializationService.Serialize(item));
        }

        private T Deserialize<T>(byte[] cachedData) =>
            NewtonSoftSerializationService.Deserialize<T>(Encoding.Default.GetString(cachedData));

        private static DistributedCacheEntryOptions GetOptions(TimeSpan? slidingExpiration)
        {
            var options = new DistributedCacheEntryOptions();
            if (slidingExpiration.HasValue)
            {
                options.SetSlidingExpiration(slidingExpiration.Value);
            }
            else
            {
                //make it persist
                //options.SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Default expiration time of 10 minutes.
            }

            return options;
        }
    }
}