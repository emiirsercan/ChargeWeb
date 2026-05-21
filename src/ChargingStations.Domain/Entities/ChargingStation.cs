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
    /// <summary>İstasyonun adı (örn: "ZES Ankara Kızılay")</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Açık adres</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Şehir (örn: "İstanbul", "Ankara")</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>İlçe (örn: "Kadıköy", "Çankaya")</summary>
    public string District { get; set; } = string.Empty;

    /// <summary>Enlem — harita üzerinde Y koordinatı</summary>
    public double Latitude { get; set; }

    /// <summary>Boylam — harita üzerinde X koordinatı</summary>
    public double Longitude { get; set; }

    /// <summary>Operatör adı (örn: "ZES", "Eşarj", "Sharz.net")</summary>
    public string OperatorName { get; set; } = string.Empty;

    /// <summary>İstasyonun genel durumu</summary>
    public StationStatus Status { get; set; } = StationStatus.Active;

    /// <summary>İstasyon hakkında ek bilgi, açıklama</summary>
    public string? Description { get; set; }

    /// <summary>İstasyonun görseli (URL)</summary>
    public string? ImageUrl { get; set; }

    /// <summary>7/24 açık mı?</summary>
    public bool IsOpen24Hours { get; set; } = true;

    /// <summary>İletişim telefonu</summary>
    public string? PhoneNumber { get; set; }

    // ── Navigation Properties ──────────────────────────────
    // EF Core bu property'leri kullanarak JOIN işlemi yapar.
    // "Bir istasyonun birden fazla soketi olabilir" ilişkisini ifade eder.

    /// <summary>Bu istasyondaki şarj soketleri</summary>
    public ICollection<Connector> Connectors { get; set; } = new List<Connector>();

    /// <summary>Bu istasyona yazılan yorumlar</summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>Bu istasyonu favorilerine ekleyen kullanıcılar</summary>
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
