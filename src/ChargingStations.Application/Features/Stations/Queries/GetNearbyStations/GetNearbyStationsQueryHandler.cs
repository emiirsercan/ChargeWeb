using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Enums;
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
            Id = s.Id,
            Name = s.Name,
            Address = s.Address,
            City = s.City,
            District = s.District,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            OperatorName = s.OperatorName,
            Status = s.Status,
            IsOpen24Hours = s.IsOpen24Hours,
            ConnectorCount = s.Connectors.Count,
            AvailableConnectorCount = s.Connectors.Count(c => c.Status == ConnectorStatus.Available),
            AverageRating = s.Reviews.Any() ? s.Reviews.Average(r => r.Rating) : 0,
            ReviewCount = s.Reviews.Count
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
