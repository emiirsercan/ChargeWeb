using ChargingStations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStations.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // ── Tablo Adı ──────────────────────────────────────────
        builder.ToTable("reviews");

        // ── Primary Key ────────────────────────────────────────
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        // ── Foreign Key'ler ────────────────────────────────────
        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.StationId).IsRequired();

        // ── Rating: 1-5 arası olmalı ───────────────────────────
        // CHECK constraint: Sadece 1-5 arası değerlere izin ver
        builder.Property(r => r.Rating)
            .IsRequired()
            .HasColumnType("integer");

        builder.ToTable(t => t.HasCheckConstraint(
            "chk_reviews_rating",
            "\"Rating\" >= 1 AND \"Rating\" <= 5"
        ));

        // ── Yorum Metni (Opsiyonel) ────────────────────────────
        builder.Property(r => r.Comment)
            .HasMaxLength(2000);

        // ── Tarihler ───────────────────────────────────────────
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(r => r.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // ── Unique Constraint ──────────────────────────────────
        // Bir kullanıcı aynı istasyona birden fazla yorum yapamaz
        builder.HasIndex(r => new { r.UserId, r.StationId })
            .IsUnique()
            .HasDatabaseName("ix_reviews_user_station_unique");

        // İstasyona göre sıralama (en yeni yorumlar) için index
        builder.HasIndex(r => new { r.StationId, r.CreatedAt })
            .HasDatabaseName("ix_reviews_station_created");
    }
}
