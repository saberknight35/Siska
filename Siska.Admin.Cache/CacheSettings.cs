namespace Siska.Admin.Cache
{
    public class CacheSettings
    {
        public bool UseDistributedCache { get; set; }
        public bool PreferRedis { get; set; }
        public string? RedisURL { get; set; }
    }
}