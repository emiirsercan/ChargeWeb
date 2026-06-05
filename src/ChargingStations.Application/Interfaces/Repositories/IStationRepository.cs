using ChargingStations.Domain.Entities;

namespace ChargingStations.Application.Interfaces.Repositories;

/// <summary>
/// Şarj istasyonu repository interface'i.
/// 
/// BU NEDEN INTERFACE?
/// Application katmanı "veriyi nasıl çekeceğini" bilmez.
/// Sadece "ne çekeceğini" tanımlar.
/// Gerçek implementasyon (EF Core + PostgreSQL) Persistence katmanında yapılır.
/// 
/// Bu sayede:
/// - Application katmanı veritabanından bağımsızdır
/// - Test yazarken mock'layabilirsin
/// - Veritabanını değiştirmek istersen sadece Persistence'ı değiştirirsin
/// </summary>
public interface IStationRepository
{
    /// <summary>Tüm istasyonları getirir (pagination destekli)</summary>
    Task<IReadOnlyList<ChargingStation>> GetAllAsync(int page, int pageSize);

    /// <summary>Id ile tek istasyon getirir (connector, review dahil)</summary>
    Task<ChargingStation?> GetByIdAsync(Guid id);

    /// <summary>Şehre göre istasyonları filtreler</summary>
    Task<IReadOnlyList<ChargingStation>> GetByCityAsync(string city, int page, int pageSize);

    /// <summary>Belirli bir konuma yakın istasyonları getirir (yarıçap bazlı)</summary>
    Task<IReadOnlyList<ChargingStation>> GetNearbyAsync(double latitude, double longitude, double radiusKm, int page, int pageSize);

    /// <summary>Arama — isim, adres veya operatör adına göre</summary>
    Task<IReadOnlyList<ChargingStation>> SearchAsync(string searchTerm, int page, int pageSize);

    /// <summary>Toplam istasyon sayısını döner (pagination için)</summary>
    Task<int> GetTotalCountAsync();

    /// <summary>Şehre göre toplam sayı</summary>
    Task<int> GetCountByCityAsync(string city);

    /// <summary>Yeni istasyon ekler</summary>
    Task<ChargingStation> AddAsync(ChargingStation station);

    /// <summary>İstasyon günceller</summary>
    Task UpdateAsync(ChargingStation station);

    /// <summary>İstasyon siler</summary>
    Task DeleteAsync(ChargingStation station);

    /// <summary>Id'nin var olup olmadığını kontrol eder</summary>
    Task<bool> ExistsAsync(Guid id);
}
