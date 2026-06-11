using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetNearbyStations;

/// <summary>
/// Yakındaki istasyonları OpenChargeMap API'den çeker.
///
/// ─── ESKİ HALİ (DB'den) ────────────────────────────────────
///   Handler → StationRepository → Supabase DB → Boş tablo!
///
/// ─── YENİ HALİ (OpenChargeMap'ten) ─────────────────────────
///   Handler → Cache var mı? → Evet → Cache'ten dön
///                            → Hayır → OpenChargeMapService → API çağrısı
///                                      → Sonucu cache'le → Dön
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Kullanıcı haritayı açtı, Kadıköy'de (40.99, 29.03), 5 km içini istiyor
///   1. Cache'e bak: "ocm:nearby:40.99:29.03:5" → yok
///   2. OpenChargeMap API'ye iste → 23 istasyon döndü
///   3. Cache'e yaz (15 dakika geçerli)
///   4. Sonucu frontend'e dön
///   5. 3 dakika sonra başka kullanıcı aynı bölgeyi sordu → cache'ten!
/// </summary>
public class GetNearbyStationsQueryHandler : IRequestHandler<GetNearbyStationsQuery, PagedResult<StationDto>>
{
    private readonly IOpenChargeMapService _ocmService;
    private readonly ICacheService _cacheService;

    public GetNearbyStationsQueryHandler(
        IOpenChargeMapService ocmService,
        ICacheService cacheService)
    {
        _ocmService = ocmService;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<StationDto>> Handle(GetNearbyStationsQuery request, CancellationToken cancellationToken)
    {
        // ── 1. Cache'e bak ──
        var cacheKey = $"ocm:nearby:{request.Latitude:F2}:{request.Longitude:F2}:{request.RadiusKm}";
        var cached = await _cacheService.GetAsync<List<StationDto>>(cacheKey);

        if (cached != null)
        {
            // Cache'te var! Sayfalama uygula ve dön
            var pagedCached = cached
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResult<StationDto>
            {
                Items = pagedCached,
                TotalCount = cached.Count,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        // ── 2. OpenChargeMap API'den çek ──
        var ocmStations = await _ocmService.GetNearbyStationsAsync(
            request.Latitude, request.Longitude, request.RadiusKm);

        // ── 3. DTO'ya dönüştür ──
        var stationDtos = ocmStations.Select(s => new StationDto
        {
            OcmId = s.OcmId,
            Name = s.Name,
            Address = s.Address,
            City = s.Town,
            District = s.StateOrProvince,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            OperatorName = s.OperatorName,
            StatusText = s.StatusType,
            ConnectorCount = s.Connections.Count,
            DistanceKm = s.DistanceKm
        }).ToList();

        // ── 4. Cache'e yaz (15 dakika) ──
        await _cacheService.SetAsync(cacheKey, stationDtos, TimeSpan.FromMinutes(15));

        // ── 5. Sayfalama uygula ve dön ──
        var paged = stationDtos
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<StationDto>
        {
            Items = paged,
            TotalCount = stationDtos.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
