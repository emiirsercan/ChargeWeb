using ChargingStations.Application.Interfaces;
using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using ChargingStations.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ChargingStations.Infrastructure.Services;

public class StationSyncService : IStationSyncService
{
    private readonly IOpenChargeMapService _ocmService;
    private readonly IStationRepository _stationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StationSyncService> _logger;

    public StationSyncService(
        IOpenChargeMapService ocmService,
        IStationRepository stationRepository,
        IUnitOfWork unitOfWork,
        ILogger<StationSyncService> logger)
    {
        _ocmService = ocmService;
        _stationRepository = stationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> SyncStationsAsync(string countryCode = "TR", int maxResults = 500)
    {
        _logger.LogInformation("Sync başlatıldı: country={Country}, max={Max}", countryCode, maxResults);

        // 1. OCM API'den istasyonları çek
        var ocmStations = await _ocmService.GetStationsByCountryAsync(countryCode, maxResults);

        if (ocmStations.Count == 0)
        {
            _logger.LogWarning("OCM API'den veri gelmedi.");
            return 0;
        }

        _logger.LogInformation("OCM'den {Count} istasyon çekildi.", ocmStations.Count);

        // 2. DB'deki mevcut istasyonları OcmId ile al
        var existingStations = await _stationRepository.GetAllByOcmIdsAsync(
            ocmStations.Select(s => s.OcmId).ToList());

        var existingSet = existingStations.Select(s => s.OcmId).ToHashSet();
        _logger.LogInformation("DB'de {Count} mevcut istasyon var.", existingSet.Count);

        // 3. Yeni istasyonları filtrele ve ekle
        int addedCount = 0;

        foreach (var ocm in ocmStations)
        {
            if (existingSet.Contains(ocm.OcmId))
                continue;

            var stationId = Guid.NewGuid();
            var station = new ChargingStation
            {
                Id = stationId,
                OcmId = ocm.OcmId,
                Name = ocm.Name,
                Address = ocm.Address,
                City = ocm.Town,
                District = string.IsNullOrEmpty(ocm.StateOrProvince) ? ocm.Town : ocm.StateOrProvince,
                Latitude = ocm.Latitude,
                Longitude = ocm.Longitude,
                OperatorName = ocm.OperatorName,
                Status = StationStatus.Active,
                IsOpen24Hours = true,
                Connectors = ocm.Connections.Select(conn => new Connector
                {
                    Id = Guid.NewGuid(),
                    StationId = stationId,
                    Type = ParseConnectorType(conn.ConnectionType),
                    PowerKW = conn.PowerKW ?? 0,
                    Status = ConnectorStatus.Available,
                    PricePerKWh = null
                }).ToList()
            };

            await _stationRepository.AddAsync(station);
            addedCount++;
        }

        // 4. Tek seferde kaydet
        if (addedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        _logger.LogInformation(
            "Sync tamamlandı: {Added} yeni istasyon eklendi, toplam {Total} OCM kaydı",
            addedCount, ocmStations.Count);

        return addedCount;
    }

    private static ConnectorType ParseConnectorType(string connectionType)
    {
        var lower = connectionType.ToLowerInvariant();

        if (lower.Contains("ccs")) return ConnectorType.CCS;
        if (lower.Contains("chademo")) return ConnectorType.CHAdeMO;
        if (lower.Contains("type 2") || lower.Contains("mennekes")) return ConnectorType.Type2;
        if (lower.Contains("type 1") || lower.Contains("j1772")) return ConnectorType.Type1;
        if (lower.Contains("tesla")) return ConnectorType.Tesla;

        return ConnectorType.Type2; // varsayılan
    }
}
