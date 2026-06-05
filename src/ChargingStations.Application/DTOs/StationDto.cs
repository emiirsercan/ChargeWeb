using ChargingStations.Domain.Enums;

namespace ChargingStations.Application.DTOs;

/// <summary>
/// İstasyon listesinde gösterilecek özet bilgiler.
/// Liste ekranında tüm detaylara gerek yok — sadece temel bilgiler yeterli.
/// 
/// DTO (Data Transfer Object) nedir?
/// Entity doğrudan frontend'e gönderilmez çünkü:
/// - Navigation property'ler circular reference oluşturur (JSON serialize patlar)
/// - Bazı alanlar gizli kalmalıdır (örn: PasswordHash)
/// - Frontend'in ihtiyacı olan veri, DB'deki veriden farklı olabilir
/// </summary>
public class StationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public StationStatus Status { get; set; }
    public bool IsOpen24Hours { get; set; }

    /// <summary>Bu istasyondaki toplam soket sayısı</summary>
    public int ConnectorCount { get; set; }

    /// <summary>Müsait soket sayısı</summary>
    public int AvailableConnectorCount { get; set; }

    /// <summary>Ortalama puan (1-5)</summary>
    public double AverageRating { get; set; }

    /// <summary>Toplam yorum sayısı</summary>
    public int ReviewCount { get; set; }
}
