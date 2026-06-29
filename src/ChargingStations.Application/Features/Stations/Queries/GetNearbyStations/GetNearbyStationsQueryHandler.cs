using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces.Repositories;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetNearbyStations;

public class GetNearbyStationsQueryHandler : IRequestHandler<GetNearbyStationsQuery, PagedResult<StationDto>>
{
    private readonly IStationRepository _stationRepository;

    public GetNearbyStationsQueryHandler(IStationRepository stationRepository)
    {
        _stationRepository = stationRepository;
    }

    public async Task<PagedResult<StationDto>> Handle(GetNearbyStationsQuery request, CancellationToken cancellationToken)
    {
        var stations = await _stationRepository.GetNearbyAsync(
            request.Latitude, request.Longitude, request.RadiusKm,
            request.Page, request.PageSize);

        var stationDtos = stations.Select(s => new StationDto
        {
            OcmId = s.OcmId,
            Name = s.Name,
            Address = s.Address,
            City = s.City,
            District = s.District,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            OperatorName = s.OperatorName,
            StatusText = s.Status.ToString(),
            ConnectorCount = s.Connectors.Count
        }).ToList();

        return new PagedResult<StationDto>
        {
            Items = stationDtos,
            TotalCount = stationDtos.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
