namespace ChargingStations.Application.DTOs;

/// <summary>
/// İstasyon listesinde gösterilecek özet bilgiler.
/// OpenChargeMap'ten gelen veriye uyumlu.
/// </summary>
public class StationDto
{
    /// <summary>OpenChargeMap ID (integer)</summary>
    public int OcmId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string OperatorName { get; set; } = string.Empty;

    /// <summary>Durum metni (Operational, Not Operational, vb.)</summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>Bağlantı noktası sayısı</summary>
    public int ConnectorCount { get; set; }

    /// <summary>Kullanıcıya olan mesafe (km) — nearby sorgusunda dolu</summary>
    public double? DistanceKm { get; set; }
}
