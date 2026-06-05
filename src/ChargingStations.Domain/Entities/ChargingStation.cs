using ChargingStations.Domain.Common;
using ChargingStations.Domain.Enums;

namespace ChargingStations.Domain.Entities;

/// Şarj istasyonu entity'si — veritabanındaki "Stations" tablosu ile eşleşir.
/// 
/// Bir istasyonun birden fazla Connector'ı (şarj soketi) olabilir.
/// Bir istasyona birden fazla Review (yorum) yazılabilir.
/// Bir istasyon birden fazla kullanıcı tarafından favorilere eklenebilir.
public class ChargingStation : BaseEntity
{
    ///İstasyonun adı 
    public string Name { get; set; } = string.Empty;

    /// Açık adres
    public string Address { get; set; } = string.Empty;

    ///Şehir
    public string City { get; set; } = string.Empty;

    ///İlçe 
    public string District { get; set; } = string.Empty;

    ///Enlem — harita üzerinde Y koordinatı
    public double Latitude { get; set; }

    ///Boylam — harita üzerinde X koordinatı
    public double Longitude { get; set; }

    public string OperatorName { get; set; } = string.Empty;

    public StationStatus Status { get; set; } = StationStatus.Active;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsOpen24Hours { get; set; } = true;

    public string? PhoneNumber { get; set; }

    // ── Navigation Properties ──────────────────────────────
    // EF Core bu property'leri kullanarak JOIN işlemi yapar.
    // "Bir istasyonun birden fazla soketi olabilir" ilişkisini ifade eder.

    /// Bu istasyondaki şarj soketleri
    public ICollection<Connector> Connectors { get; set; } = new List<Connector>();

    /// Bu istasyona yazılan yorumlar
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// Bu istasyonu favorilerine ekleyen kullanıcılar
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
