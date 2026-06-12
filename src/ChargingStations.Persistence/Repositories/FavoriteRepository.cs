using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using ChargingStations.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChargingStations.Persistence.Repositories;
public class FavoriteRepository : IFavoriteRepository
{
    private readonly AppDbContext _context;

    public FavoriteRepository(AppDbContext context)
    {
        _context = context;
    }

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

    public async Task<bool> ExistsAsync(Guid userId, Guid stationId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.StationId == stationId);
    }
    public async Task<Favorite?> GetAsync(Guid userId, Guid stationId)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.StationId == stationId);
    }
}
