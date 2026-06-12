namespace ChargingStations.Domain.Enums;
public enum StationStatus
{
    /// <summary>Aktif ve çalışıyor</summary>
    Active = 0,

    /// <summary>Bakımda — geçici olarak kullanılamaz</summary>
    Maintenance = 1,

    /// <summary>Devre dışı — kalıcı olarak kapalı</summary>
    Inactive = 2,

    /// <summary>Yakında açılacak — henüz hizmete girmedi</summary>
    ComingSoon = 3
}
