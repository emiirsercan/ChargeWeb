using ChargingStations.Domain.Common;
using ChargingStations.Domain.Enums;

namespace ChargingStations.Domain.Entities;

/// <summary>
/// Şarj soketi (connector) entity'si.
/// 
/// Her istasyonda 1 veya daha fazla soket bulunur.
/// Her soketin bir tipi (Type2, CCS vs.), gücü (kW) ve fiyatı vardır.
/// 
/// Örnek: Bir ZES istasyonunda 2x CCS (50kW) + 1x Type2 (22kW) olabilir.
/// </summary>
public class Connector : BaseEntity
{
    /// <summary>Hangi istasyona ait (Foreign Key)</summary>
    public Guid StationId { get; set; }

    /// <summary>Soket tipi (Type2, CCS, CHAdeMO vs.)</summary>
    public ConnectorType Type { get; set; }

    /// <summary>Şarj gücü (kW cinsinden). Örnek: 22, 50, 150, 350</summary>
    public double PowerKW { get; set; }

    /// <summary>Soketin anlık durumu</summary>
    public ConnectorStatus Status { get; set; } = ConnectorStatus.Available;

    /// <summary>kWh başına fiyat (TL). Nullable çünkü bazı istasyonlar ücretsiz.</summary>
    public decimal? PricePerKWh { get; set; }

    // ── Navigation Property ──────────────────────────────
    /// <summary>Bu soketin ait olduğu istasyon</summary>
    public ChargingStation Station { get; set; } = null!;
}
