using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetAllStations;

/// <summary>
/// Tüm istasyonları listeler — OpenChargeMap'ten ülke bazlı çeker.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// React frontend yüklendi → "Türkiye'deki istasyonları göster"
///   1. Cache'e bak: "ocm:country:TR" → yok
///   2. OpenChargeMap API → countrycode=TR → 500+ istasyon
///   3. Cache'e yaz (30 dakika)
///   4. Sayfalayarak dön (page=1, pageSize=20)
/// </summary>
public class GetAllStationsQueryHandler : IRequestHandler<GetAllStationsQuery, PagedResult<StationDto>>
{
    private readonly IOpenChargeMapService _ocmService;
    private readonly ICacheService _cacheService;

    public GetAllStationsQueryHandler(
        IOpenChargeMapService ocmService,
        ICacheService cacheService)
    {
        _ocmService = ocmService;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<StationDto>> Handle(GetAllStationsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = "ocm:country:TR";
        var cached = await _cacheService.GetAsync<List<StationDto>>(cacheKey);

        if (cached != null)
        {
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

        // OpenChargeMap'ten Türkiye'deki istasyonları çek
        var ocmStations = await _ocmService.GetStationsByCountryAsync("TR", maxResults: 500);

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

        // 30 dakika cache'le (ülke verileri sık değişmez)
        await _cacheService.SetAsync(cacheKey, stationDtos, TimeSpan.FromMinutes(30));

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
