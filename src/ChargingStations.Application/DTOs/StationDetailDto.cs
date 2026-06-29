namespace ChargingStations.Application.DTOs;

public class StationDetailDto
{
    public Guid Id { get; set; }
    public int OcmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;

    /// Ortalama kullanıcı puanı (1-5)
    public double AverageRating { get; set; }

    /// Toplam yorum sayısı
    public int ReviewCount { get; set; }

    /// Bu istasyondaki tüm connector'lar
    public List<ConnectorDto> Connectors { get; set; } = new();
}
