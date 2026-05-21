namespace ChargingStations.Domain.Common;

/// Tüm entity'lerin miras aldığı temel sınıf.
/// 
/// Her entity'nin:
/// - Benzersiz bir Id'si (Guid)
/// - Oluşturulma tarihi
/// - Güncellenme tarihi
/// olmalıdır.
/// 
/// Guid kullanmamızın sebebi:
/// - Veritabanına gitmeden önce Id oluşturabilirsin (int'te bunu yapamazsın)
/// - Distributed sistemlerde çakışma riski yok
/// - URL'de tahmin edilemez (güvenlik)
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
