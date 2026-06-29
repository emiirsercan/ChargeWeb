using ChargingStations.Application.Interfaces;
using ChargingStations.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChargingStations.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // ── TokenService: JWT bileklik üreticisi ──
        services.AddScoped<ITokenService, TokenService>();

        // ── CacheService: Bellek not defteri ──
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();
        services.AddHttpClient<IOpenChargeMapService, OpenChargeMapService>(client =>
        {
            client.BaseAddress = new Uri("https://api.openchargemap.io/v3/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // ── StationSyncService: OCM → DB senkronizasyon ──
        services.AddScoped<IStationSyncService, StationSyncService>();

        return services;
    }
}
