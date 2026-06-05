using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using ChargingStations.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChargingStations.Persistence.Repositories;

/// <summary>
/// IReviewRepository'nin EF Core implementasyonu.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// İstasyon detay sayfasında "Yorumlar" bölümü var.
/// Kullanıcı sayfayı açıyor:
///   1. GetByStationIdAsync() → İstasyona ait son yorumları getir
///   2. GetAverageRatingAsync() → "4.3 ★" hesapla
///   3. Kullanıcı kendi yorumunu gönderiyor:
///      a. UserHasReviewedAsync() → false → AddAsync() → kaydet
///      b. UserHasReviewedAsync() → true  → "Zaten yorum yaptınız" hatası
/// </summary>
public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Bir istasyona ait yorumları sayfalı getirir.
    /// Yorumla birlikte kullanıcı adı da dahil edilir.
    /// </summary>
    public async Task<IReadOnlyList<Review>> GetByStationIdAsync(Guid stationId, int page, int pageSize)
    {
        return await _context.Reviews
            .Include(r => r.User) // Yorum yapan kişinin adını göster
            .Where(r => r.StationId == stationId)
            .OrderByDescending(r => r.CreatedAt) // Yeni yorumlar üstte
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Review>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Reviews
            .Include(r => r.Station) // Hangi istasyon için yazıldı?
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Station)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Review> AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        return review;
    }

    public Task DeleteAsync(Review review)
    {
        _context.Reviews.Remove(review);
        return Task.CompletedTask;
    }

    public async Task<bool> UserHasReviewedAsync(Guid userId, Guid stationId)
    {
        return await _context.Reviews
            .AnyAsync(r => r.UserId == userId && r.StationId == stationId);
    }

    /// <summary>
    /// İstasyonun ortalama puanını hesaplar.
    ///
    /// SQL: SELECT AVG(Rating) FROM reviews WHERE station_id = @stationId
    ///
    /// Yorum yoksa 0 döner (GetValueOrDefault).
    /// </summary>
    public async Task<double> GetAverageRatingAsync(Guid stationId)
    {
        var avg = await _context.Reviews
            .Where(r => r.StationId == stationId)
            .AverageAsync(r => (double?)r.Rating);

        return avg.GetValueOrDefault(0);
    }
}
