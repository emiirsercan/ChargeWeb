using ChargingStations.Application.Interfaces;
using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Commands.CreateStation;

public class CreateStationCommandHandler : IRequestHandler<CreateStationCommand, Guid>
{
    private readonly IStationRepository _stationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStationCommandHandler(IStationRepository stationRepository, IUnitOfWork unitOfWork)
    {
        _stationRepository = stationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateStationCommand request, CancellationToken cancellationToken)
    {
        // Command → Domain Entity'ye dönüştür
        var station = new ChargingStation
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            District = request.District,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            OperatorName = request.OperatorName,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            IsOpen24Hours = request.IsOpen24Hours,
            PhoneNumber = request.PhoneNumber,
            Connectors = request.Connectors.Select(c => new Connector
            {
                Id = Guid.NewGuid(),
                Type = c.Type,
                PowerKW = c.PowerKW,
                PricePerKWh = c.PricePerKWh
            }).ToList()
        };

        await _stationRepository.AddAsync(station);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return station.Id;
    }
}
