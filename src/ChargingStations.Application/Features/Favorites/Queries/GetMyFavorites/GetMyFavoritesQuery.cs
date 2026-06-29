using ChargingStations.Application.DTOs;
using MediatR;

namespace ChargingStations.Application.Features.Favorites.Queries.GetMyFavorites;

public class GetMyFavoritesQuery : IRequest<List<StationDto>>
{
    public Guid UserId { get; set; } // JWT'den set edilecek
}
