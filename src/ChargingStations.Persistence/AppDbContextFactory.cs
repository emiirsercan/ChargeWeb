using ChargingStations.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ChargingStations.Persistence;

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

