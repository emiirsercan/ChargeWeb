using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Enums;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetStationById;

public class GetStationByIdQueryHandler : IRequestHandler<GetStationByIdQuery, StationDetailDto?>
{
    private readonly IStationRepository _stationRepository;

    public GetStationByIdQueryHandler(IStationRepository stationRepository)
    {
        _stationRepository = stationRepository;
    }

    public async Task<StationDetailDto?> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
    {
        var station = await _stationRepository.GetByIdAsync(request.Id);

        if (station is null)
            return null;

        return new StationDetailDto
        {
            Id = station.Id,
            Name = station.Name,
            Address = station.Address,
            City = station.City,
            District = station.District,
            Latitude = station.Latitude,
            Longitude = station.Longitude,
            OperatorName = station.OperatorName,
            Status = station.Status,
            Description = station.Description,
            ImageUrl = station.ImageUrl,
            IsOpen24Hours = station.IsOpen24Hours,
            PhoneNumber = station.PhoneNumber,
            CreatedAt = station.CreatedAt,
            AverageRating = station.Reviews.Any() ? station.Reviews.Average(r => r.Rating) : 0,
            ReviewCount = station.Reviews.Count,
            Connectors = station.Connectors.Select(c => new ConnectorDto
            {
                Id = c.Id,
                Type = c.Type,
                TypeName = GetConnectorTypeName(c.Type),
                PowerKW = c.PowerKW,
                Status = c.Status,
                StatusName = GetConnectorStatusName(c.Status),
                PricePerKWh = c.PricePerKWh
            }).ToList(),
            RecentReviews = station.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserFullName = r.User?.FullName ?? "Anonim",
                    StationId = r.StationId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList()
        };
    }

    private static string GetConnectorTypeName(ConnectorType type) => type switch
    {
        ConnectorType.Type1 => "Type 1 (AC)",
        ConnectorType.Type2 => "Type 2 (AC)",
        ConnectorType.CCS => "CCS (DC Hızlı Şarj)",
        ConnectorType.CHAdeMO => "CHAdeMO (DC)",
        ConnectorType.Tesla => "Tesla Supercharger",
        _ => type.ToString()
    };

    private static string GetConnectorStatusName(ConnectorStatus status) => status switch
    {
        ConnectorStatus.Available => "Müsait",
        ConnectorStatus.InUse => "Kullanımda",
        ConnectorStatus.OutOfService => "Servis Dışı",
        _ => status.ToString()
    };
}
