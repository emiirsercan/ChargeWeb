namespace ChargingStations.Application.DTOs;
public class ConnectorDto
{
    /// <summary>Connector tipi (örn: "CCS (Type 2)", "CHAdeMO", "Type 2")</summary>
    public string ConnectionType { get; set; } = string.Empty;

    /// <summary>Güç (kW)</summary>
    public double? PowerKW { get; set; }

    /// <summary>Akım tipi (AC / DC)</summary>
    public string CurrentType { get; set; } = string.Empty;

    /// <summary>Bu tipten kaç adet var</summary>
    public int? Quantity { get; set; }

    /// <summary>Durum</summary>
    public string Status { get; set; } = string.Empty;
}
