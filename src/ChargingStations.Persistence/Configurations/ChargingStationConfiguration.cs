using ChargingStations.Domain.Entities;
using ChargingStations.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStations.Persistence.Configurations;

/// <summary>
/// ChargingStation entity'sinin veritabanı yapılandırması.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// EF Core bize soruyor: "ChargingStation sınıfını tabloya nasıl çevireyim?"
/// Biz de burada şunu diyoruz:
///   - Tablo adı "stations" olsun
///   - Name kolonu max 200 karakter, zorunlu olsun
///   - Latitude/Longitude birlikte index'lensin (hızlı yakın arama için)
///   - Connector'larla ilişki: "Bir istasyon silinirse connector'ları da silinsin"
/// </summary>
public class ChargingStationConfiguration : IEntityTypeConfiguration<ChargingStation>
{
    public void Configure(EntityTypeBuilder<ChargingStation> builder)
    {
        // ── Tablo Adı ──────────────────────────────────────────
        builder.ToTable("stations");

        // ── Primary Key ────────────────────────────────────────
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever(); // Guid'i koddan üretiyoruz

        // ── Zorunlu Alanlar ────────────────────────────────────
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.District)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.OperatorName)
            .IsRequired()
            .HasMaxLength(200);

        // ── Opsiyonel Alanlar ──────────────────────────────────
        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.ImageUrl)
            .HasMaxLength(500);

        builder.Property(s => s.PhoneNumber)
            .HasMaxLength(20);

        // ── Konum Alanları ─────────────────────────────────────
        builder.Property(s => s.Latitude)
            .IsRequired()
            .HasColumnType("double precision");

        builder.Property(s => s.Longitude)
            .IsRequired()
            .HasColumnType("double precision");

        // ── Status Enum ────────────────────────────────────────
        // PostgreSQL native enum yerine string olarak saklıyoruz.
        // Neden? Supabase dashboard'da okunması kolay, migration'da sorun çıkmaz.
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        // ── Tarihler ───────────────────────────────────────────
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // ── Index'ler ──────────────────────────────────────────
        // Şehre göre filtreleme sık yapılacak → City'ye index
        builder.HasIndex(s => s.City)
            .HasDatabaseName("ix_stations_city");

        // Yakın istasyon sorgusu için lat/lng index
        // (PostGIS olmadan Haversine formulü kullanacağız)
        builder.HasIndex(s => new { s.Latitude, s.Longitude })
            .HasDatabaseName("ix_stations_location");

        // Status'a göre filtreleme için
        builder.HasIndex(s => s.Status)
            .HasDatabaseName("ix_stations_status");

        // ── İlişkiler ──────────────────────────────────────────
        // Bir istasyonun birçok Connector'ı var
        builder.HasMany(s => s.Connectors)
            .WithOne(c => c.Station)
            .HasForeignKey(c => c.StationId)
            .OnDelete(DeleteBehavior.Cascade); // İstasyon silinince connector'lar da silinir

        // Bir istasyonun birçok Review'ı var
        builder.HasMany(s => s.Reviews)
            .WithOne(r => r.Station)
            .HasForeignKey(r => r.StationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Bir istasyon birçok kullanıcının favorisinde
        builder.HasMany(s => s.Favorites)
            .WithOne(f => f.Station)
            .HasForeignKey(f => f.StationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
