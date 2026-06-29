using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces.Repositories;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetStationById;

public class GetStationByIdQueryHandler : IRequestHandler<GetStationByIdQuery, StationDetailDto?>
{
    private readonly IStationRepository _stationRepository;
    private readonly IReviewRepository _reviewRepository;

    public GetStationByIdQueryHandler(
        IStationRepository stationRepository,
        IReviewRepository reviewRepository)
    {
        _stationRepository = stationRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<StationDetailDto?> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
    {
        var station = await _stationRepository.GetByIdAsync(request.Id);

        if (station is null)
            throw new KeyNotFoundException($"İstasyon bulunamadı: {request.Id}");

        var avgRating = await _reviewRepository.GetAverageRatingAsync(station.Id);

        return new StationDetailDto
        {
            Id = station.Id,
            OcmId = station.OcmId,
            Name = station.Name,
            Address = station.Address,
            City = station.City,
            District = station.District,
            Latitude = station.Latitude,
            Longitude = station.Longitude,
            OperatorName = station.OperatorName,
            StatusText = station.Status.ToString(),
            AverageRating = avgRating,
            ReviewCount = station.Reviews.Count,
            Connectors = station.Connectors.Select(c => new ConnectorDto
            {
                ConnectionType = c.Type.ToString(),
                PowerKW = c.PowerKW,
                CurrentType = c.PowerKW >= 50 ? "DC" : "AC",
                Quantity = 1,
                Status = c.Status.ToString()
            }).ToList()
        };
    }
}
