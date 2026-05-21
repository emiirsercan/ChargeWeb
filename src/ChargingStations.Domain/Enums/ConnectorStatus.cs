namespace ChargingStations.Domain.Enums;

/// <summary>
/// Tek bir şarj soketinin anlık durumu.
/// </summary>
public enum ConnectorStatus
{
    /// <summary>Müsait — şarj yapılabilir</summary>
    Available = 0,

    /// <summary>Kullanımda — başka araç şarj oluyor</summary>
    InUse = 1,

    /// <summary>Arızalı — servis dışı</summary>
    OutOfService = 2
}
