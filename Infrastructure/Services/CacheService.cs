using System.Text;
using Infrastructure.Contracts;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    private readonly IDistributedCache _cache = cache;
    
    public Task Add(string key, string value)
    {
        return _cache.SetAsync(key, Encoding.UTF8.GetBytes(value));
    }

    public Task Delete(string key)
    {
        return _cache.RemoveAsync(key);
    }

    public Task<string?> Get(string key)
    {
        return _cache.GetStringAsync(key);
    }
}