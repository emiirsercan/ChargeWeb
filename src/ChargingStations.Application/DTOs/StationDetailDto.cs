namespace ChargingStations.Application.DTOs;

/// <summary>
/// İstasyon detay sayfası — connector'lar dahil tüm bilgiler.
/// OpenChargeMap'ten gelen veriye uyumlu.
/// </summary>
public class StationDetailDto
{
    public int OcmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string UsageType { get; set; } = string.Empty;
    public DateTime? DateLastStatusUpdate { get; set; }

    /// <summary>Bu istasyondaki tüm connector'lar</summary>
    public List<ConnectorDto> Connectors { get; set; } = new();
}
