using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Enums;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetAllStations;

/// <summary>
/// GetAllStationsQuery'yi işleyen handler.
/// Repository'den verileri çeker, DTO'ya dönüştürür ve döner.
/// </summary>
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
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
