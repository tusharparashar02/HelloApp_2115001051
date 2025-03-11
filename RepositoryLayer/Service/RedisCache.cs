using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RepositoryLayer.Entity;
using StackExchange.Redis;


namespace RepositoryLayer.Service
{
    public class RedisCache
    {
        private readonly IDatabase _cache;
        private string Key = "MyUsers";

        public RedisCache(IConnectionMultiplexer cache)
        {
            _cache = cache.GetDatabase();
        }

        public void SaveCache( List<Entity.UserEntity> Users)
        {
            _cache.StringSet(Key, JsonSerializer.Serialize(Users), TimeSpan.FromMinutes(10));

            return;
        }

        public string GetData()
        {
            var data = _cache.StringGet(Key);
            return data;
        }
    }
}
