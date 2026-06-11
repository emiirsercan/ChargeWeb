using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ChargingStations.Application;

/// <summary>
/// Application katmanının DI (Dependency Injection) kayıtları.
/// 
/// Bu metod API katmanındaki Program.cs'den çağrılır.
/// Her katman kendi servislerini kaydetmekten sorumludur.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR — tüm Command/Query handler'larını otomatik bulur ve kaydeder
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Pipeline Behavior: Request → [Validation] → Handler
            // Her request handler'a gitmeden önce validator'dan geçer
            cfg.AddOpenBehavior(typeof(Behaviors.ValidationBehavior<,>));
        });

        // FluentValidation — tüm Validator sınıflarını otomatik bulur
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
