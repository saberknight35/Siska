using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Siska.Admin.Cache
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();

            if (settings != null && settings!.UseDistributedCache)
            {
                if (settings.PreferRedis)
                {
                    _ = services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = settings.RedisURL;
                        options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                        {
                            AbortOnConnectFail = false,
                            EndPoints = { settings.RedisURL }
                        };
                    });
                }
                else
                {
                    _ = services.AddDistributedMemoryCache();
                }

                _ = services.AddSingleton<ICacheService, DistributedCacheService>();
            }
            else
            {
                _ = services.AddMemoryCache()
                    .AddSingleton<ICacheService, LocalCacheService>();
            }

            return services;
        }
    }
}
