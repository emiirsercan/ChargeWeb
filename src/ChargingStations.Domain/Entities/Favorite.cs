using ChargingStations.Domain.Common;

namespace ChargingStations.Domain.Entities;

/// <summary>
/// Favori entity'si — kullanıcı ile istasyon arasındaki many-to-many ilişkiyi temsil eder.
/// 
/// Neden ayrı bir entity? 
/// - CreatedAt bilgisini tutmak istiyoruz (ne zaman favorilere eklendi)
/// - İleride ek alanlar eklenebilir (bildirim tercihi vs.)
/// - EF Core'da explicit join table daha kontrollü
/// </summary>
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
