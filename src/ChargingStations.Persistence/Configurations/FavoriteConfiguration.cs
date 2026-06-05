using ChargingStations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStations.Persistence.Configurations;

/// <summary>
/// Favorite entity'sinin veritabanı yapılandırması.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Bu tablo aslında bir "köprü tablo" (junction table).
/// User ve ChargingStation arasındaki Many-to-Many ilişkiyi temsil eder.
///
/// Neden ayrı bir entity yaptık, EF Core'un otomatik M-to-M yerine?
///   → Çünkü "ne zaman favorilere eklendi?" bilgisini (CreatedAt)
///     tutmak istiyoruz. Otomatik junction table bunu desteklemez.
///
/// Örnek veri:
///   UserId = Emre'nin Id'si, StationId = ZES Şişli Id'si → Favori var
///   UserId = Emre'nin Id'si, StationId = Eşarj Kadıköy Id'si → Favori var
///   Emre'nin 2 favori istasyonu var demektir.
/// </summary>
public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        // ── Tablo Adı ──────────────────────────────────────────
        builder.ToTable("favorites");

        // ── Primary Key ────────────────────────────────────────
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();

        // ── Foreign Key'ler ────────────────────────────────────
        builder.Property(f => f.UserId).IsRequired();
        builder.Property(f => f.StationId).IsRequired();

        // ── Tarihler ───────────────────────────────────────────
        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(f => f.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // ── Unique Constraint ──────────────────────────────────
        // Bir kullanıcı aynı istasyonu sadece bir kez favorilere ekleyebilir
        builder.HasIndex(f => new { f.UserId, f.StationId })
            .IsUnique()
            .HasDatabaseName("ix_favorites_user_station_unique");

        // Kullanıcının tüm favorilerini hızlı getirmek için
        builder.HasIndex(f => f.UserId)
            .HasDatabaseName("ix_favorites_user_id");
    }
}
