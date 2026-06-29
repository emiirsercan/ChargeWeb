namespace ChargingStations.Application.Interfaces;

public interface IStationSyncService
{
    /// OpenChargeMap'ten istasyonları çekip DB'ye kaydeder (upsert)
    Task<int> SyncStationsAsync(string countryCode = "TR", int maxResults = 500);
}
