using ChargingStations.Domain.Common;

namespace ChargingStations.Domain.Entities;

public class Review : BaseEntity
{
    /// <summary>Yorumu yazan kullanıcı (FK)</summary>
    public Guid UserId { get; set; }

    /// <summary>Yorum yapılan istasyon (FK)</summary>
    public Guid StationId { get; set; }

    /// <summary>Puan (1-5 arası)</summary>
    public int Rating { get; set; }

    /// <summary>Yorum metni (opsiyonel)</summary>
    public string? Comment { get; set; }

    // ── Navigation Properties ──────────────────────────────
    /// <summary>Yorumu yazan kullanıcı</summary>
    public User User { get; set; } = null!;

    /// <summary>Yorum yapılan istasyon</summary>
    public ChargingStation Station { get; set; } = null!;
}
