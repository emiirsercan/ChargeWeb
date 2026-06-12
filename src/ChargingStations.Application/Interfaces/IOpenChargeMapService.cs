namespace ChargingStations.Application.Interfaces;
public interface IOpenChargeMapService
{
    Task<List<OpenChargeMapStation>> GetNearbyStationsAsync(
        double latitude, double longitude, double radiusKm, int maxResults = 50);
    Task<List<OpenChargeMapStation>> GetStationsByCountryAsync(
        string countryCode, int maxResults = 100);
    Task<OpenChargeMapStation?> GetStationByIdAsync(int ocmId);
}
public class OpenChargeMapStation
{
    /// <summary>OpenChargeMap'teki benzersiz ID (integer)</summary>
    public int OcmId { get; set; }

    /// <summary>İstasyon adı (örn: "ZES Sultanahmet")</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Adres satırı</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Şehir / İlçe</summary>
    public string Town { get; set; } = string.Empty;

    /// <summary>İl / Eyalet</summary>
    public string StateOrProvince { get; set; } = string.Empty;

    /// <summary>Ülke adı</summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>Enlem (latitude)</summary>
    public double Latitude { get; set; }

    /// <summary>Boylam (longitude)</summary>
    public double Longitude { get; set; }

    /// <summary>Kullanıcının konumuna olan mesafe (km)</summary>
    public double? DistanceKm { get; set; }

    /// <summary>Operatör / Şebeke adı (örn: "ZES", "Eşarj", "Togg")</summary>
    public string OperatorName { get; set; } = string.Empty;

    /// <summary>Kullanım tipi (Public, Private, vb.)</summary>
    public string UsageType { get; set; } = string.Empty;

    /// <summary>Durum (Operational, Not Operational, vb.)</summary>
    public string StatusType { get; set; } = string.Empty;

    /// <summary>Bağlantı noktaları (connector'lar)</summary>
    public List<OpenChargeMapConnection> Connections { get; set; } = new();

    /// <summary>Son güncelleme tarihi</summary>
    public DateTime? DateLastStatusUpdate { get; set; }
}


public class OpenChargeMapConnection
{
    /// <summary>Connector tipi (örn: "CCS (Type 2)", "CHAdeMO", "Type 2")</summary>
    public string ConnectionType { get; set; } = string.Empty;

    /// <summary>Güç (kW) — örn: 50, 150, 350</summary>
    public double? PowerKW { get; set; }

    /// <summary>Akım tipi — AC veya DC</summary>
    public string CurrentType { get; set; } = string.Empty;

    /// <summary>Bu tipten kaç adet var</summary>
    public int? Quantity { get; set; }

    /// <summary>Durum</summary>
    public string Status { get; set; } = string.Empty;
}
