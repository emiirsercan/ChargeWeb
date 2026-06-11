using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetStationById;

/// <summary>
/// Tek istasyonun detayını OpenChargeMap'ten çeker.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Kullanıcı haritada bir istasyona tıkladı → ID: 54321
///   1. Cache'e bak: "ocm:station:54321" → yok
///   2. OpenChargeMap API → chargepointid=54321
///   3. Detaylı bilgi geldi (connector'lar dahil)
///   4. Cache'e yaz (10 dakika)
///   5. Frontend'e dön
/// </summary>
public class GetStationByIdQueryHandler : IRequestHandler<GetStationByIdQuery, StationDetailDto?>
{
    private readonly IOpenChargeMapService _ocmService;
    private readonly ICacheService _cacheService;

    public GetStationByIdQueryHandler(
        IOpenChargeMapService ocmService,
        ICacheService cacheService)
    {
        _ocmService = ocmService;
        _cacheService = cacheService;
    }

    public async Task<StationDetailDto?> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"ocm:station:{request.OcmId}";
        var cached = await _cacheService.GetAsync<StationDetailDto>(cacheKey);

        if (cached != null)
            return cached;

        var ocmStation = await _ocmService.GetStationByIdAsync(request.OcmId);

        if (ocmStation is null)
            throw new KeyNotFoundException($"İstasyon bulunamadı: {request.OcmId}");

        var detail = new StationDetailDto
        {
            OcmId = ocmStation.OcmId,
            Name = ocmStation.Name,
            Address = ocmStation.Address,
            City = ocmStation.Town,
            District = ocmStation.StateOrProvince,
            Country = ocmStation.Country,
            Latitude = ocmStation.Latitude,
            Longitude = ocmStation.Longitude,
            OperatorName = ocmStation.OperatorName,
            StatusText = ocmStation.StatusType,
            UsageType = ocmStation.UsageType,
            DateLastStatusUpdate = ocmStation.DateLastStatusUpdate,
            Connectors = ocmStation.Connections.Select(c => new ConnectorDto
            {
                ConnectionType = c.ConnectionType,
                PowerKW = c.PowerKW,
                CurrentType = c.CurrentType,
                Quantity = c.Quantity,
                Status = c.Status
            }).ToList()
        };

        // 10 dakika cache'le
        await _cacheService.SetAsync(cacheKey, detail, TimeSpan.FromMinutes(10));

        return detail;
    }
}
