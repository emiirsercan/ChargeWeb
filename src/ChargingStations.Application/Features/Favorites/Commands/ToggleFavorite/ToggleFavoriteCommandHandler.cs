using ChargingStations.Application.Interfaces;
using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using MediatR;

namespace ChargingStations.Application.Features.Favorites.Commands.ToggleFavorite;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, ToggleFavoriteResult>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IStationRepository _stationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleFavoriteCommandHandler(
        IFavoriteRepository favoriteRepository,
        IStationRepository stationRepository,
        IUnitOfWork unitOfWork)
    {
        _favoriteRepository = favoriteRepository;
        _stationRepository = stationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ToggleFavoriteResult> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        // İstasyon var mı kontrol et
        if (!await _stationRepository.ExistsAsync(request.StationId))
            throw new KeyNotFoundException("İstasyon bulunamadı.");

        var existing = await _favoriteRepository.GetAsync(request.UserId, request.StationId);

        if (existing != null)
        {
            // Zaten favoride → çıkar
            await _favoriteRepository.DeleteAsync(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ToggleFavoriteResult
            {
                IsFavorited = false,
                Message = "Favorilerden çıkarıldı."
            };
        }

        // Favoride değil → ekle
        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            StationId = request.StationId
        };

        await _favoriteRepository.AddAsync(favorite);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ToggleFavoriteResult
        {
            IsFavorited = true,
            Message = "Favorilere eklendi."
        };
    }
}
