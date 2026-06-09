using System.Collections.Concurrent;
using ChargingStations.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ChargingStations.Infrastructure.Services;

/// <summary>
/// In-Memory Cache Servisi — "Garsonun Not Defteri"
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Bir restoranda garson düşün. Müşteriler sürekli soruyor:
///   "Bugünün menüsü ne?"
///   "Bugünün menüsü ne?"
///   "Bugünün menüsü ne?"
///
/// Garson her seferinde mutfağa gidip şefe sormak yerine:
///   1. İlk sorulduğunda mutfağa gider, menüyü öğrenir
///   2. Not defterine yazar: "Günün menüsü: Adana Kebap" (5 dk geçerli)
///   3. Sonraki 5 dakika boyunca not defterinden cevap verir
///   4. 5 dakika dolunca not'u siler, tekrar mutfağa gider
///
/// Bu sınıf o "not defteri"dir:
///   - Mutfak = PostgreSQL veritabanı (Supabase)
///   - Not defteri = RAM (sunucunun belleği)
///   - 5 dakika = cache expiration süresi
///
/// ─── NEDEN IN-MEMORY, NEDEN REDİS DEĞİL? ──────────────────
/// In-Memory: Tek sunucu için yeterli, hızlı, ücretsiz
/// Redis: Birden fazla sunucu varsa gerekli (distributed cache)
///
/// Şimdilik tek sunucumuz var → In-Memory yeterli.
/// İleride ölçeklenirsek → ICacheService interface'i sayesinde
/// sadece bu dosyayı RedisCacheService ile değiştirmek yeterli.
/// Application katmanını hiç değiştirmeyiz! (Dependency Inversion)
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    // Hangi key'lerin cache'de olduğunu takip etmek için
    // RemoveByPrefixAsync'te "stations:" ile başlayan tüm key'leri silmek istediğimizde
    // hangi key'lerin var olduğunu bilmemiz lazım
    private readonly ConcurrentDictionary<string, bool> _cacheKeys = new();

    public InMemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Cache'ten veri okur. Yoksa null döner.
    ///
    /// Senaryo:
    ///   key = "stations:page:1:size:10"
    ///   → Cache'te varsa: Anında döner (0ms)
    ///   → Cache'te yoksa: null döner, handler veritabanına gider
    /// </summary>
    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    /// <summary>
    /// Cache'e veri yazar.
    ///
    /// Senaryo:
    ///   key = "stations:page:1:size:10"
    ///   value = [500 istasyon listesi]
    ///   expiration = 5 dakika
    ///
    ///   → 5 dakika boyunca bu veri bellekte tutulur
    ///   → Sonraki GET isteklerinde veritabanına gitmeden bellekten döner
    /// </summary>
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

    /// <summary>
    /// Cache'ten tek bir key siler.
    ///
    /// Senaryo: Yeni istasyon eklendi
    ///   → "stations:page:1:size:10" cache'i artık geçersiz
    ///   → Bu metot ile sil
    ///   → Sonraki istekte güncel veri veritabanından gelir
    /// </summary>
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _cacheKeys.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Belirli prefix ile başlayan TÜM key'leri siler (cache invalidation).
    ///
    /// Senaryo: Admin yeni bir istasyon ekledi
    ///   → Cache'te şunlar var:
    ///     "stations:page:1:size:10"
    ///     "stations:page:2:size:10"
    ///     "stations:nearby:41.01:28.97"
    ///     "stations:detail:abc-123"
    ///   → RemoveByPrefixAsync("stations") çağrıldı
    ///   → Hepsi silindi! Çünkü hepsi "stations" ile başlıyor
    ///   → Sonraki isteklerde güncel veriler veritabanından gelir
    /// </summary>
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
