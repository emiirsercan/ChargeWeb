using ChargingStations.Application.DTOs;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetStationById;

public record GetStationByIdQuery(Guid Id) : IRequest<StationDetailDto?>;
