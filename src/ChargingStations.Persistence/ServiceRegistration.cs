using ChargingStations.Application.Interfaces;
using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Persistence.Context;
using ChargingStations.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChargingStations.Persistence;
public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── DbContext Kaydı ────────────────────────────────────
        // AppDbContext'i PostgreSQL (Npgsql) ile yapılandır
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    // Migration assembly: dotnet ef migration komutunun
                    // hangi projede çalıştığını belirtir
                    npgsqlOptions.MigrationsAssembly("ChargingStations.Persistence");

                    // Geçici bağlantı hatalarında otomatik tekrar dene
                    // (Supabase cold start vs. için faydalı)
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null
                    );
                }
            );
        });

        // ── Repository Kayıtları ───────────────────────────────
        // Interface → Implementation eşleştirmeleri
        // Handler'lar interface'e bağımlı, implementasyonu bilmez
        services.AddScoped<IStationRepository, StationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        // ── Unit of Work ───────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
