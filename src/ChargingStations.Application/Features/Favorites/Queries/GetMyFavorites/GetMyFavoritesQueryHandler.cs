using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces.Repositories;
using MediatR;

namespace ChargingStations.Application.Features.Favorites.Queries.GetMyFavorites;

public class GetMyFavoritesQueryHandler : IRequestHandler<GetMyFavoritesQuery, List<StationDto>>
{
    private readonly IFavoriteRepository _favoriteRepository;

    public GetMyFavoritesQueryHandler(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<List<StationDto>> Handle(GetMyFavoritesQuery request, CancellationToken cancellationToken)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(request.UserId);

        return favorites.Select(f => new StationDto
        {
            OcmId = f.Station.OcmId,
            Name = f.Station.Name,
            Address = f.Station.Address,
            City = f.Station.City,
            District = f.Station.District,
            Latitude = f.Station.Latitude,
            Longitude = f.Station.Longitude,
            OperatorName = f.Station.OperatorName,
            StatusText = f.Station.Status.ToString(),
            ConnectorCount = f.Station.Connectors.Count
        }).ToList();
    }
}
