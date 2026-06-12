using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using ChargingStations.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChargingStations.Persistence.Repositories;

public class StationRepository : IStationRepository
{
    private readonly AppDbContext _context;

    public StationRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<ChargingStation>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Stations
            .Include(s => s.Connectors)  // JOIN connectors
            .Include(s => s.Reviews)     // JOIN reviews
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize) // OFFSET
            .Take(pageSize)              // LIMIT
            .AsNoTracking()              // Performans: salt okunur, takip etme
            .ToListAsync();
    }
    public async Task<ChargingStation?> GetByIdAsync(Guid id)
    {
        return await _context.Stations
            .Include(s => s.Connectors)
            .Include(s => s.Reviews)
                .ThenInclude(r => r.User) // Yorum yapan kullanıcının adını da çek
            .Include(s => s.Favorites)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<IReadOnlyList<ChargingStation>> GetByCityAsync(string city, int page, int pageSize)
    {
        return await _context.Stations
            .Include(s => s.Connectors)
            .Include(s => s.Reviews)
            .Where(s => s.City.ToUpper() == city.ToUpper())
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ChargingStation>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        int page,
        int pageSize)
    {
        // 1 derece enlem ≈ 111 km
        var latDelta = radiusKm / 111.0;
        // 1 derece boylam ≈ 111 * cos(enlem) km
        var lngDelta = radiusKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0));

        // Adım 1: Bounding box ile SQL'de kaba filtre
        var candidates = await _context.Stations
            .Include(s => s.Connectors)
            .Include(s => s.Reviews)
            .Where(s =>
                s.Latitude >= latitude - latDelta &&
                s.Latitude <= latitude + latDelta &&
                s.Longitude >= longitude - lngDelta &&
                s.Longitude <= longitude + lngDelta)
            .AsNoTracking()
            .ToListAsync();

        // Adım 2: C#'da kesin Haversine hesabı
        return candidates
            .Where(s => CalculateDistanceKm(latitude, longitude, s.Latitude, s.Longitude) <= radiusKm)
            .OrderBy(s => CalculateDistanceKm(latitude, longitude, s.Latitude, s.Longitude))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    /// <summary>
    /// İsim, adres veya operatör adına göre arama.
    ///
    /// SQL: WHERE name ILIKE '%zes%' OR address ILIKE '%zes%'
    /// (ILIKE = case-insensitive LIKE, PostgreSQL'e özgü)
    /// </summary>
    public async Task<IReadOnlyList<ChargingStation>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        var term = searchTerm.ToLower();

        return await _context.Stations
            .Include(s => s.Connectors)
            .Include(s => s.Reviews)
            .Where(s =>
                s.Name.ToLower().Contains(term) ||
                s.Address.ToLower().Contains(term) ||
                s.OperatorName.ToLower().Contains(term) ||
                s.City.ToLower().Contains(term))
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Stations.CountAsync();
    }

    public async Task<int> GetCountByCityAsync(string city)
    {
        return await _context.Stations
            .CountAsync(s => s.City.ToUpper() == city.ToUpper());
    }

    public async Task<ChargingStation> AddAsync(ChargingStation station)
    {
        await _context.Stations.AddAsync(station);
        return station;
        // Not: SaveChanges burada çağrılmıyor!
        // UnitOfWork pattern: kayıt işlemini Handler koordine eder
        // Handler başarılı olursa _unitOfWork.SaveChangesAsync() çağırır
    }

    public Task UpdateAsync(ChargingStation station)
    {
        _context.Stations.Update(station);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ChargingStation station)
    {
        _context.Stations.Remove(station);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Stations.AnyAsync(s => s.Id == id);
    }

    // ── Yardımcı Metod: Haversine Formülü ──────────────────────
    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0; // Dünya yarıçapı (km)

        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }
}
