using ChargingStations.Application.DTOs;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetStationById;

/// <summary>
/// Id ile tek istasyon detayı getirme sorgusu.
/// Connector'lar ve son yorumlar dahil.
/// </summary>
public class GetStationByIdQuery : IRequest<StationDetailDto?>
{
    public Guid Id { get; set; }

    public GetStationByIdQuery(Guid id)
    {
        Id = id;
    }
}
