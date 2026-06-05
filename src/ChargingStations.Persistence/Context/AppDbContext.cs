using ChargingStations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChargingStations.Persistence.Context;

/// <summary>
/// EF Core'un veritabanıyla konuştuğu merkezi sınıf.
/// 
/// ─── NEDEN DbContext? ───────────────────────────────────────
/// EF Core'da her veritabanı işlemi (SELECT, INSERT, UPDATE, DELETE)
/// DbContext üzerinden yapılır. DbContext şu işleri yapar:
/// 
///   1. Bağlantı Yönetimi  → PostgreSQL'e bağlanır/bağlantıyı kapatır
///   2. Change Tracking    → Hangi entity'lerin değiştiğini takip eder
///   3. Query Translation  → C# LINQ sorgularını SQL'e çevirir
///   4. Migration          → Schema değişikliklerini yönetir
/// 
/// ─── DbSet NEDİR? ───────────────────────────────────────────
/// DbSet<T> = Veritabanındaki bir tablo.
/// DbSet<ChargingStation> → "stations" tablosunu temsil eder.
/// DbSet<User> → "users" tablosunu temsil eder.
/// 
/// ─── FLUENT API NEREDE? ────────────────────────────────────
/// OnModelCreating → EF Core'a "tablolar nasıl yapılandırılsın?"
/// sorusunu soran yerdir. Ayrıntılar Configuration klasöründe.
/// </summary>
public class AppDbContext : DbContext
{
    // ── DbSet'ler (Tablolar) ────────────────────────────────────
    public DbSet<ChargingStation> Stations { get; set; }
    public DbSet<Connector> Connectors { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    // ── Constructor ─────────────────────────────────────────────
    // options → connection string, provider (Npgsql) gibi ayarları taşır.
    // Bu ayarlar Dependency Injection (DI) sistemi tarafından enjekte edilir.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // ── Model Yapılandırması ────────────────────────────────────
    // EF Core bu metodu uygulama ilk açıldığında çağırır.
    // Burada "hangi sınıf hangi tabloya, hangi alan hangi kolona karşılık gelir"
    // gibi kurallar tanımlanır.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Assembly'deki tüm IEntityTypeConfiguration<T> sınıflarını
        // otomatik olarak uygular. Her entity için ayrı Configuration
        // dosyası yazdığımızda burası otomatik bulacak.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    // ── SaveChanges Override ────────────────────────────────────
    // Her kayıt öncesinde UpdatedAt alanını otomatik set eder.
    // Böylece her entity'de manuel UpdatedAt güncellemek zorunda kalmayız.
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Değiştirilmiş (Modified) entity'lerin UpdatedAt'ini güncelle
        foreach (var entry in ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified))
        {
            if (entry.Entity is ChargingStations.Domain.Common.BaseEntity baseEntity)
            {
                baseEntity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
