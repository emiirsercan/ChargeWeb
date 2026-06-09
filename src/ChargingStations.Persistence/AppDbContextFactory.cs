using ChargingStations.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ChargingStations.Persistence;

/// <summary>
/// Migration oluşturma sırasında DbContext'i nasıl yaratacağını EF Core'a söyler.
///
/// ─── NEDEN GEREKLİ? ────────────────────────────────────────
/// "dotnet ef migrations add" komutu çalıştığında:
///   1. Uygulama tam olarak başlamaz (Program.cs çalışmaz)
///   2. Sadece DbContext'i oluşturması lazım
///   3. DI Container yok, connection string nereden okuyacak?
///
/// IDesignTimeDbContextFactory → "EF araçlarına özel fabrika"
/// Bu fabrika sadece migration komutlarında kullanılır, runtime'da değil.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// "dotnet ef migrations add InitialCreate" komutu çalışınca:
///   1. EF, AppDbContextFactory'yi bulur (IDesignTimeDbContextFactory impl.)
///   2. CreateDbContext() çağırır
///   3. appsettings.json'u okur, connection string'i alır
///   4. AppDbContext oluşturur
///   5. Entity Configuration'larını tarar
///   6. Migration dosyasını (SQL komutlarını) üretir
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Session Pooler (IPv4) — doğrudan bağlantı
        // Migration aracı için güvenli, runtime'da appsettings.json kullanılır
        optionsBuilder.UseNpgsql(
            "Host=aws-1-ap-southeast-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.yhmjqlxtthzherewwbiy;Password=190501.Emirr;SSL Mode=Require;Trust Server Certificate=true",
            npgsql => npgsql.MigrationsAssembly("ChargingStations.Persistence")
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}

