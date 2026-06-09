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
        // Scoped: Her HTTP isteği için yeni bir instance
        // Neden Scoped? → Her request kendi token servisini kullansın
        // Singleton olsa da olurdu ama Scoped daha güvenli
        services.AddScoped<ITokenService, TokenService>();

        // ── CacheService: Bellek not defteri ──
        // AddMemoryCache(): .NET'in built-in IMemoryCache'ini aktif eder
        // Singleton: Tüm uygulama boyunca tek bir cache instance
        // Neden Singleton? → Cache zaten paylaşılan bir kaynak,
        // herkes aynı not defterini okumalı
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        return services;
    }
}
