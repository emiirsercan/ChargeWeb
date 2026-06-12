using ChargingStations.Domain.Common;

namespace ChargingStations.Domain.Entities;

public class Favorite : BaseEntity
{
    /// <summary>Favoriyi ekleyen kullanıcı (FK)</summary>
    public Guid UserId { get; set; }

    /// <summary>Favoriye eklenen istasyon (FK)</summary>
    public Guid StationId { get; set; }

    // ── Navigation Properties ──────────────────────────────
    public User User { get; set; } = null!;
    public ChargingStation Station { get; set; } = null!;
}
