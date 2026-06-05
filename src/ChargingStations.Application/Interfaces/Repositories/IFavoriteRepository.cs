using ChargingStations.Domain.Entities;

namespace ChargingStations.Application.Interfaces.Repositories;

/// <summary>
/// Favori repository interface'i.
/// </summary>
public interface IFavoriteRepository
{
    /// <summary>Kullanıcının tüm favori istasyonlarını getirir</summary>
    Task<IReadOnlyList<Favorite>> GetByUserIdAsync(Guid userId);

    /// <summary>Favorilere ekler</summary>
    Task<Favorite> AddAsync(Favorite favorite);

    /// <summary>Favorilerden çıkarır</summary>
    Task DeleteAsync(Favorite favorite);

    /// <summary>Bu istasyon zaten favorilerde mi?</summary>
    Task<bool> ExistsAsync(Guid userId, Guid stationId);

    /// <summary>UserId + StationId ile favoriyi bul</summary>
    Task<Favorite?> GetAsync(Guid userId, Guid stationId);
}
