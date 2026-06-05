using ChargingStations.Domain.Entities;

namespace ChargingStations.Application.Interfaces.Repositories;

/// <summary>
/// Yorum repository interface'i.
/// </summary>
public interface IReviewRepository
{
    /// <summary>Bir istasyona ait tüm yorumları getirir</summary>
    Task<IReadOnlyList<Review>> GetByStationIdAsync(Guid stationId, int page, int pageSize);

    /// <summary>Bir kullanıcının tüm yorumlarını getirir</summary>
    Task<IReadOnlyList<Review>> GetByUserIdAsync(Guid userId);

    Task<Review?> GetByIdAsync(Guid id);
    Task<Review> AddAsync(Review review);
    Task DeleteAsync(Review review);

    /// <summary>Kullanıcı bu istasyona daha önce yorum yapmış mı?</summary>
    Task<bool> UserHasReviewedAsync(Guid userId, Guid stationId);

    /// <summary>İstasyonun ortalama puanını hesaplar</summary>
    Task<double> GetAverageRatingAsync(Guid stationId);
}
