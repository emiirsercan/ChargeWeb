namespace ChargingStations.Domain.Enums;
public enum ConnectorType
{
    /// <summary>AC yavaş şarj — eski model, ABD standardı</summary>
    Type1 = 0,

    /// <summary>AC şarj — Avrupa/Türkiye standardı (en yaygın)</summary>
    Type2 = 1,

    /// <summary>DC hızlı şarj — Avrupa/Türkiye standardı (Combined Charging System)</summary>
    CCS = 2,

    /// <summary>DC hızlı şarj — Japon standardı (Nissan Leaf vs.)</summary>
    CHAdeMO = 3,

    /// <summary>Tesla'nın kendi şarj sistemi</summary>
    Tesla = 4
}
