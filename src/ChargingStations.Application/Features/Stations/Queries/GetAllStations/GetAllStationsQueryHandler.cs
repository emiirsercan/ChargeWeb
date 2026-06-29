using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces.Repositories;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetAllStations;

public class GetAllStationsQueryHandler : IRequestHandler<GetAllStationsQuery, PagedResult<StationDto>>
{
    private readonly IStationRepository _stationRepository;

    public GetAllStationsQueryHandler(IStationRepository stationRepository)
    {
        _stationRepository = stationRepository;
    }

    public async Task<PagedResult<StationDto>> Handle(GetAllStationsQuery request, CancellationToken cancellationToken)
    {
        var stations = await _stationRepository.GetAllAsync(request.Page, request.PageSize);
        var totalCount = await _stationRepository.GetTotalCountAsync();

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
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
