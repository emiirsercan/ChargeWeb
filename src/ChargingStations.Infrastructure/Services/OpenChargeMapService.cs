using System.Net.Http.Json;
using System.Text.Json;
using ChargingStations.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChargingStations.Infrastructure.Services;
public class OpenChargeMapService : IOpenChargeMapService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenChargeMapService> _logger;

    private const string BaseUrl = "https://api.openchargemap.io/v3/poi";

    // JSON parse ayarları — API'den gelen property isimleri case-insensitive
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenChargeMapService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenChargeMapService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Koordinatlara yakın istasyonları getirir.
    ///
    /// Senaryo: Kullanıcı Kadıköy'de (40.99, 29.03), 5 km içini istiyor
    ///   → API'ye istek: ?latitude=40.99&longitude=29.03&distance=5
    ///   → JSON cevap → DTO'lara dönüştür → Handler'a dön
    /// </summary>
    public async Task<List<OpenChargeMapStation>> GetNearbyStationsAsync(
        double latitude, double longitude, double radiusKm, int maxResults = 50)
    {
        try
        {
            var apiKey = _configuration["OpenChargeMap:ApiKey"] ?? "";
            var url = $"{BaseUrl}?output=json&latitude={latitude}&longitude={longitude}" +
                      $"&distance={radiusKm}&distanceunit=KM" +
                      $"&maxresults={maxResults}&compact=true&verbose=false" +
                      $"&key={apiKey}";

            _logger.LogInformation(
                "OpenChargeMap API çağrısı: lat={Lat}, lng={Lng}, radius={Radius}km",
                latitude, longitude, radiusKm);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var rawStations = JsonSerializer.Deserialize<List<OcmRawPoi>>(jsonString, JsonOptions);

            return rawStations?.Select(MapToStation).ToList() ?? new List<OpenChargeMapStation>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenChargeMap API hatası (nearby): {Message}", ex.Message);
            return new List<OpenChargeMapStation>();
        }
    }

    /// <summary>
    /// Ülke koduna göre istasyonları getirir.
    /// countryCode: "TR" → Türkiye, "DE" → Almanya
    /// </summary>
    public async Task<List<OpenChargeMapStation>> GetStationsByCountryAsync(
        string countryCode, int maxResults = 100)
    {
        try
        {
            var apiKey = _configuration["OpenChargeMap:ApiKey"] ?? "";
            var url = $"{BaseUrl}?output=json&countrycode={countryCode}" +
                      $"&maxresults={maxResults}&compact=true&verbose=false" +
                      $"&key={apiKey}";

            _logger.LogInformation("OpenChargeMap API çağrısı: country={Country}", countryCode);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var rawStations = JsonSerializer.Deserialize<List<OcmRawPoi>>(jsonString, JsonOptions);

            return rawStations?.Select(MapToStation).ToList() ?? new List<OpenChargeMapStation>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenChargeMap API hatası (country): {Message}", ex.Message);
            return new List<OpenChargeMapStation>();
        }
    }

    /// <summary>
    /// Tek istasyonun detayını getirir.
    /// </summary>
    public async Task<OpenChargeMapStation?> GetStationByIdAsync(int ocmId)
    {
        try
        {
            var apiKey = _configuration["OpenChargeMap:ApiKey"] ?? "";
            var url = $"{BaseUrl}?output=json&chargepointid={ocmId}" +
                      $"&compact=true&verbose=false&key={apiKey}";

            _logger.LogInformation("OpenChargeMap API çağrısı: id={Id}", ocmId);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var rawStations = JsonSerializer.Deserialize<List<OcmRawPoi>>(jsonString, JsonOptions);

            var raw = rawStations?.FirstOrDefault();
            return raw != null ? MapToStation(raw) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenChargeMap API hatası (byId): {Message}", ex.Message);
            return null;
        }
    }

    // ══════════════════════════════════════════════════════════════
    // ── HAM JSON → TEMİZ DTO DÖNÜŞÜMÜ ──────────────────────────
    // OpenChargeMap'in JSON yapısı çok karmaşık (iç içe nesneler).
    // Bu metot "çirkin" JSON'ı "temiz" DTO'ya çevirir.
    //
    // Ham JSON:
    //   { "AddressInfo": { "Title": "ZES", "Latitude": 41.01 }, ... }
    //
    // Temiz DTO:
    //   { Name: "ZES", Latitude: 41.01, ... }
    // ══════════════════════════════════════════════════════════════
    private static OpenChargeMapStation MapToStation(OcmRawPoi raw)
    {
        var station = new OpenChargeMapStation
        {
            OcmId = raw.ID,
            Name = raw.AddressInfo?.Title ?? "Bilinmeyen İstasyon",
            Address = raw.AddressInfo?.AddressLine1 ?? "",
            Town = raw.AddressInfo?.Town ?? "",
            StateOrProvince = raw.AddressInfo?.StateOrProvince ?? "",
            Country = raw.AddressInfo?.Country?.Title ?? "",
            Latitude = raw.AddressInfo?.Latitude ?? 0,
            Longitude = raw.AddressInfo?.Longitude ?? 0,
            DistanceKm = raw.AddressInfo?.Distance,
            OperatorName = raw.OperatorInfo?.Title ?? "Bilinmeyen Operatör",
            StatusType = raw.StatusType?.Title ?? "Bilinmiyor",
            UsageType = raw.UsageType?.Title ?? "Bilinmiyor",
            DateLastStatusUpdate = raw.DateLastStatusUpdate
        };

        if (raw.Connections != null)
        {
            station.Connections = raw.Connections.Select(c => new OpenChargeMapConnection
            {
                ConnectionType = c.ConnectionType?.Title ?? "Bilinmeyen",
                PowerKW = c.PowerKW,
                CurrentType = c.CurrentType?.Title ?? "",
                Quantity = c.Quantity,
                Status = c.StatusType?.Title ?? ""
            }).ToList();
        }

        return station;
    }
}

// ══════════════════════════════════════════════════════════════════
// ── OpenChargeMap HAM JSON MODELLERİ ────────────────────────────
// Bu sınıflar API'den gelen JSON'ı deserialize etmek için kullanılır.
// Dış dünyaya açılmaz — sadece bu dosyada kullanılır.
// "Kirli" veriyi "temiz" DTO'ya çevirmek için aracı modeller.
// ══════════════════════════════════════════════════════════════════

internal class OcmRawPoi
{
    public int ID { get; set; }
    public string? UUID { get; set; }
    public OcmAddressInfo? AddressInfo { get; set; }
    public OcmOperatorInfo? OperatorInfo { get; set; }
    public OcmRefData? UsageType { get; set; }
    public OcmRefData? StatusType { get; set; }
    public List<OcmConnection>? Connections { get; set; }
    public DateTime? DateLastStatusUpdate { get; set; }
    public DateTime? DateCreated { get; set; }
    public int? NumberOfPoints { get; set; }
}

internal class OcmAddressInfo
{
    public string? Title { get; set; }
    public string? AddressLine1 { get; set; }
    public string? Town { get; set; }
    public string? StateOrProvince { get; set; }
    public string? Postcode { get; set; }
    public OcmCountry? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Distance { get; set; }
}

internal class OcmCountry
{
    public string? ISOCode { get; set; }
    public string? Title { get; set; }
}

internal class OcmOperatorInfo
{
    public string? Title { get; set; }
    public string? WebsiteURL { get; set; }
}

internal class OcmConnection
{
    public OcmRefData? ConnectionType { get; set; }
    public OcmRefData? CurrentType { get; set; }
    public OcmRefData? StatusType { get; set; }
    public double? PowerKW { get; set; }
    public int? Quantity { get; set; }
}

internal class OcmRefData
{
    public int ID { get; set; }
    public string? Title { get; set; }
}
