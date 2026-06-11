namespace ChargingStations.Application.Interfaces;

/// <summary>
/// OpenChargeMap dış API servisi interface'i.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Düşün ki bir çeviri bürosu açtın. Müşterilerin sana Türkçe
/// metin veriyor, sen İngilizce'ye çevirtip geri veriyorsun.
/// 
/// Ama çevirmeni kendin değilsin — Google Translate API'yi kullanıyorsun.
/// Müşteri Google'ı bilmiyor, sadece senin büronla muhatap oluyor.
///
/// Burada da aynı mantık:
///   Frontend → Bizim API → OpenChargeMap API → Cevap → Frontend
///                  ↑
///           Müşteri sadece bizi tanır
///
/// Application katmanı "bir yerden istasyon verileri gelecek" diyor,
/// AMA nereden geldiğini bilmiyor (OpenChargeMap mı, Google Maps mı?).
/// Bu bilgi sadece Infrastructure katmanında.
/// </summary>
public interface IOpenChargeMapService
{
    /// <summary>
    /// Belirli koordinatlara yakın şarj istasyonlarını getirir.
    ///
    /// Senaryo: Kullanıcı İstanbul Kadıköy'de, 5 km içindeki istasyonları istiyor
    ///   latitude: 40.9923, longitude: 29.0295, radiusKm: 5
    ///   → OpenChargeMap API'ye istek at → Sonuçları dön
    /// </summary>
    Task<List<OpenChargeMapStation>> GetNearbyStationsAsync(
        double latitude, double longitude, double radiusKm, int maxResults = 50);

    /// <summary>
    /// Ülke koduna göre istasyonları getirir.
    ///
    /// Senaryo: Türkiye'deki tüm istasyonları listele
    ///   countryCode: "TR" → OpenChargeMap'ten Türkiye verisi çek
    /// </summary>
    Task<List<OpenChargeMapStation>> GetStationsByCountryAsync(
        string countryCode, int maxResults = 100);

    /// <summary>
    /// Tek bir istasyonun detayını ID ile getirir.
    ///
    /// Senaryo: Kullanıcı haritada bir istasyona tıkladı
    ///   ocmId: 12345 → O istasyonun detayını getir
    /// </summary>
    Task<OpenChargeMapStation?> GetStationByIdAsync(int ocmId);
}

/// <summary>
/// OpenChargeMap API'den dönen istasyon verisi.
/// API'nin JSON cevabından parse edilen "temizlenmiş" model.
///
/// API'den gelen ham JSON çok karmaşık (50+ alan).
/// Biz sadece ihtiyacımız olan alanları alıyoruz.
/// </summary>
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

/// <summary>
/// İstasyondaki bağlantı noktası (connector) bilgisi.
/// Bir istasyonda birden fazla connector olabilir (CCS2, CHAdeMO, Type2 vb.)
/// </summary>
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
