using ChargingStations.Application.DTOs;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetStationById;

/// <summary>
/// OpenChargeMap ID ile tek istasyon detayı getirme sorgusu.
/// OCM'de ID'ler integer (örn: 12345), Guid değil.
/// </summary>
public class GetStationByIdQuery : IRequest<StationDetailDto?>
{
    public int OcmId { get; set; }

    public GetStationByIdQuery(int ocmId)
    {
        OcmId = ocmId;
    }
}
