using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using ChargingStations.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChargingStations.Persistence.Repositories;

/// <summary>
/// IFavoriteRepository'nin EF Core implementasyonu.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Kullanıcı "Emre" uygulamada bir istasyonu favorilere ekliyor.
///
/// Olası akışlar:
///   EKLE:  ExistsAsync() → false → AddAsync() → SaveChanges → 201 Created
///   SİL:   ExistsAsync() → true  → GetAsync() → DeleteAsync() → SaveChanges → 200 OK
///   TEKRAR: ExistsAsync() → true  → 409 Conflict (zaten favorilerde)
///
/// Favorite tablosundaki unique constraint (ix_favorites_user_station_unique)
/// bunu veritabanı seviyesinde de güvence altına alır.
/// </summary>
public class FavoriteRepository : IFavoriteRepository
{
    private readonly AppDbContext _context;

    public FavoriteRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Kullanıcının tüm favori istasyonlarını getirir.
    /// Station bilgileri de dahil edilir (liste gösterimi için).
    /// </summary>
    public async Task<IReadOnlyList<Favorite>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Favorites
            .Include(f => f.Station)
                .ThenInclude(s => s.Connectors) // Favori listesinde soket sayısını göstermek için
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt) // En son eklenen en üstte
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Favorite> AddAsync(Favorite favorite)
    {
        await _context.Favorites.AddAsync(favorite);
        return favorite;
    }

    public Task DeleteAsync(Favorite favorite)
    {
        _context.Favorites.Remove(favorite);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Bu istasyon kullanıcının favorilerinde mi?
    /// Toggle butonu için önce bu kontrol yapılır.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid userId, Guid stationId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.StationId == stationId);
    }

    /// <summary>
    /// Favoriyi silmek için önce entity'i bul.
    /// (Remove() için entity instance lazım)
    /// </summary>
    public async Task<Favorite?> GetAsync(Guid userId, Guid stationId)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.StationId == stationId);
    }
}
