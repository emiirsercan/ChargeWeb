using ChargingStations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStations.Persistence.Configurations;

public class ConnectorConfiguration : IEntityTypeConfiguration<Connector>
{
    public void Configure(EntityTypeBuilder<Connector> builder)
    {
        // ── Tablo Adı ──────────────────────────────────────────
        builder.ToTable("connectors");

        // ── Primary Key ────────────────────────────────────────
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        // ── Foreign Key ────────────────────────────────────────
        builder.Property(c => c.StationId)
            .IsRequired();

        // ── Enum'lar → String olarak sakla ─────────────────────
        // ConnectorType: "Type2", "CCS", "CHAdeMO", "Tesla"
        builder.Property(c => c.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        // ConnectorStatus: "Available", "InUse", "Faulted", "OutOfService"
        builder.Property(c => c.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        // ── Sayısal Alanlar ────────────────────────────────────
        builder.Property(c => c.PowerKW)
            .IsRequired()
            .HasColumnType("double precision");

        // PricePerKWh nullable — bazı istasyonlar ücretsiz!
        // Örnek: Bazı AVM'ler müşterilerine ücretsiz şarj sunuyor
        builder.Property(c => c.PricePerKWh)
            .HasColumnType("numeric(10,2)"); // Para birimi için precision gerekli

        // ── Tarihler ───────────────────────────────────────────
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(c => c.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // ── Index'ler ──────────────────────────────────────────
        // "Bu istasyondaki müsait soket var mı?" sorgusu için
        builder.HasIndex(c => new { c.StationId, c.Status })
            .HasDatabaseName("ix_connectors_station_status");

        // Connector tipine göre filtreleme
        builder.HasIndex(c => c.Type)
            .HasDatabaseName("ix_connectors_type");
    }
}
