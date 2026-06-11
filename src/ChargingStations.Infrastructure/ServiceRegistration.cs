using ChargingStations.Application.Interfaces;
using ChargingStations.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChargingStations.Infrastructure;

/// <summary>
/// Infrastructure katmanının DI (Dependency Injection) kayıt noktası.
///
/// ─── SENARYO: "Binanın Teknik Oda Kurulumu" ────────────────
/// Bir binanın teknik odasını düşün. Buraya tüm sistemler bağlanır:
///   - Güvenlik kamerası sistemi → TokenService (kimlik doğrulama)
///   - Klima sistemi → CacheService (performans optimizasyonu)
///
/// Bu sınıf, Program.cs'te çağrılır:
///   builder.Services.AddInfrastructureServices(builder.Configuration);
///
/// Program.cs bu detayları bilmez:
///   "Cache In-Memory mi, Redis mi?" → Bilmez, bilmesine gerek yok
///   "Token HS256 mı, RS256 mı?" → Bilmez, bilmesine gerek yok
///
/// Bu bilgi sadece Infrastructure katmanında kalır.
/// Clean Architecture'ın gücü bu!
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // ── TokenService: JWT bileklik üreticisi ──
        services.AddScoped<ITokenService, TokenService>();

        // ── CacheService: Bellek not defteri ──
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        // ── OpenChargeMapService: Dış API istemcisi ──
        // AddHttpClient: HttpClient'ı DI üzerinden yönetir
        // → Socket exhaustion (bağlantı tükenmesi) sorununu önler
        // → Her seferinde "new HttpClient()" yazmak KÖTÜ pratiktir!
        // → .NET HttpClientFactory bunu bizim için yönetir
        services.AddHttpClient<IOpenChargeMapService, OpenChargeMapService>(client =>
        {
            client.BaseAddress = new Uri("https://api.openchargemap.io/v3/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
