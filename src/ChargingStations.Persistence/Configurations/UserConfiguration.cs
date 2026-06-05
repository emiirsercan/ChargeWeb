using ChargingStations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargingStations.Persistence.Configurations;

/// <summary>
/// User entity'sinin veritabanı yapılandırması.
///
/// ─── GÜVENLİK NOTU ─────────────────────────────────────────
/// PasswordHash alanı her zaman BCrypt ile hash'lenmiş şifreyi tutar.
/// Veritabanında hiçbir zaman plain-text şifre bulunmaz.
///
/// Örnek:
///   Gerçek şifre: "Abc123!"
///   Veritabanındaki: "$2a$11$xnQ8gY..." (60 karakter BCrypt hash)
///
/// Bu hash'ten geriye şifreyi çözmek matematiksel olarak imkânsızdır.
/// (BCrypt intentionally slow'dur — brute force'a karşı koruma)
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // ── Tablo Adı ──────────────────────────────────────────
        builder.ToTable("users");

        // ── Primary Key ────────────────────────────────────────
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        // ── Email: Unique Index ─────────────────────────────────
        // Her kullanıcının email'i eşsiz olmalı — aynı email ile 2 hesap açılamaz
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email_unique");

        // ── Ad Soyad ───────────────────────────────────────────
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        // ── Şifre Hash'i ───────────────────────────────────────
        // BCrypt hash'i her zaman 60 karakter uzunluğunda
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255); // BCrypt: 60, biraz pay bıraktık

        // ── Rol ────────────────────────────────────────────────
        // UserRole.User, UserRole.Admin, UserRole.StationOwner
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        // ── Tarihler ───────────────────────────────────────────
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // ── İlişkiler ──────────────────────────────────────────
        // Bir kullanıcının birçok yorumu var
        builder.HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Bir kullanıcının birçok favorisi var
        builder.HasMany(u => u.Favorites)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
