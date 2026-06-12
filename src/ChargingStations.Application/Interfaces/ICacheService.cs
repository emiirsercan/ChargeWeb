namespace ChargingStations.Application.Interfaces;
public interface ICacheService
{
    /// <summary>Cache'ten veri okur. Yoksa null döner.</summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>Cache'e veri yazar (belirli süre için).</summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>Cache'ten siler.</summary>
    Task RemoveAsync(string key);

    /// <summary>Belirli prefix ile başlayan tüm key'leri siler (invalidation).</summary>
    Task RemoveByPrefixAsync(string prefix);
}
