using ChargingStations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChargingStations.Persistence.Context;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

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
