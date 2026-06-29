using MediatR;

namespace ChargingStations.Application.Features.Favorites.Commands.ToggleFavorite;

public class ToggleFavoriteCommand : IRequest<ToggleFavoriteResult>
{
    public Guid StationId { get; set; }
    public Guid UserId { get; set; } // JWT'den set edilecek
}

public class ToggleFavoriteResult
{
    public bool IsFavorited { get; set; }
    public string Message { get; set; } = string.Empty;
}
