using ChargingStations.Domain.Enums;

namespace ChargingStations.Application.DTOs;

/// <summary>
/// Soket bilgisi — istasyon detayında gösterilecek.
/// </summary>
public class ConnectorDto
{
    public Guid Id { get; set; }
    public ConnectorType Type { get; set; }

    /// <summary>Soket tipinin kullanıcı dostu adı (örn: "CCS (DC Hızlı Şarj)")</summary>
    public string TypeName { get; set; } = string.Empty;

    public double PowerKW { get; set; }
    public ConnectorStatus Status { get; set; }

    /// <summary>Durumun kullanıcı dostu adı (örn: "Müsait", "Kullanımda")</summary>
    public string StatusName { get; set; } = string.Empty;

    public decimal? PricePerKWh { get; set; }
}
