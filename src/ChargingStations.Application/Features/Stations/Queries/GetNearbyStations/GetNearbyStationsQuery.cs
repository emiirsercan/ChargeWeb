using ChargingStations.Application.DTOs;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetNearbyStations;

/// <summary>
/// Yakındaki istasyonları getirme sorgusu.
/// Kullanıcının konumuna göre belirli yarıçaptaki istasyonları döner.
/// </summary>
public class GetNearbyStationsQuery : IRequest<PagedResult<StationDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10.0;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
