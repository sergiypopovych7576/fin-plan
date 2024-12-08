using FP.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace FP.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<T> Get<T>(string key)
        {
            return default;
            var value = await _db.StringGetAsync(key);
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            else
            {
                return JsonSerializer.Deserialize<T>(value);
            }
        }

        public async Task Set(string key, object value, int minutes)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, TimeSpan.FromMinutes(minutes));
        }

        public Task Reset(string key)
        {
            return _db.KeyDeleteAsync(key);
        }
    }
}
