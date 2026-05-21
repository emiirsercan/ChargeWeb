namespace ChargingStations.Domain.Enums;

/// <summary>
/// Kullanıcı rolleri.
/// </summary>
public enum UserRole
{
    /// <summary>Normal kullanıcı — yorum yapabilir, favori ekleyebilir</summary>
    User = 0,

    /// <summary>Admin — istasyon CRUD, kullanıcı yönetimi</summary>
    Admin = 1,

    /// <summary>İstasyon sahibi — kendi istasyonlarını yönetebilir</summary>
    StationOwner = 2
}
