using System.Collections.Concurrent;
using ChargingStations.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ChargingStations.Infrastructure.Services;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, bool> _cacheKeys = new();

    public InMemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
        {
            // AbsoluteExpirationRelativeToNow: "Şu andan itibaren X süre sonra sil"
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        else
        {
            // Varsayılan: 5 dakika
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        }

        _cache.Set(key, value, options);
        _cacheKeys.TryAdd(key, true);

        return Task.CompletedTask;
    }
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _cacheKeys.TryRemove(key, out _);
        return Task.CompletedTask;
    }
    public Task RemoveByPrefixAsync(string prefix)
    {
        var keysToRemove = _cacheKeys.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _cacheKeys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }
}
