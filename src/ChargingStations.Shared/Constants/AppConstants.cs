namespace ChargingStations.Shared.Constants;

/// <summary>
/// Uygulama genelinde kullanılan sabitler.
/// Magic string/number kullanımını önler.
/// </summary>
public static class AppConstants
{
    /// <summary>Varsayılan sayfalama boyutu</summary>
    public const int DefaultPageSize = 20;

    /// <summary>Maksimum sayfalama boyutu</summary>
    public const int MaxPageSize = 100;

    /// <summary>Yakınlık araması varsayılan yarıçap (km)</summary>
    public const double DefaultSearchRadiusKm = 10.0;

    /// <summary>Maksimum yakınlık araması yarıçapı (km)</summary>
    public const double MaxSearchRadiusKm = 100.0;

    /// <summary>Minimum yorum puanı</summary>
    public const int MinRating = 1;

    /// <summary>Maksimum yorum puanı</summary>
    public const int MaxRating = 5;

    /// <summary>Cache key prefix'leri</summary>
    public static class CacheKeys
    {
        public const string AllStations = "stations:all";
        public const string StationById = "stations:id:";
        public const string StationsByCity = "stations:city:";
        public const string NearbyStations = "stations:nearby:";
    }

    /// <summary>Cache süreleri (dakika)</summary>
    public static class CacheDurations
    {
        public const int ShortMinutes = 5;
        public const int MediumMinutes = 30;
        public const int LongMinutes = 60;
    }
}
