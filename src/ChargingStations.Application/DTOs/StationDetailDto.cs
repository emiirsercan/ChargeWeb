using ChargingStations.Domain.Enums;

namespace ChargingStations.Application.DTOs;

/// <summary>
/// İstasyon detay sayfası için — soketler ve yorumlar dahil tüm bilgiler.
/// </summary>
public class StationDetailDto
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
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsOpen24Hours { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>Ortalama puan</summary>
    public double AverageRating { get; set; }

    /// <summary>Toplam yorum sayısı</summary>
    public int ReviewCount { get; set; }

    /// <summary>Bu istasyondaki tüm soketler</summary>
    public List<ConnectorDto> Connectors { get; set; } = new();

    /// <summary>Son yorumlar</summary>
    public List<ReviewDto> RecentReviews { get; set; } = new();
}
