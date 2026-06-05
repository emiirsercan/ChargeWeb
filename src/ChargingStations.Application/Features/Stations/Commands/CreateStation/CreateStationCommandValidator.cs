using FluentValidation;

namespace ChargingStations.Application.Features.Stations.Commands.CreateStation;

/// <summary>
/// CreateStationCommand doğrulama kuralları.
/// 
/// FluentValidation ile her alan için kural tanımlanır.
/// Bu validator MediatR pipeline'ında otomatik çalışır —
/// kuralsız veri Handler'a ulaşamaz.
/// </summary>
public class CreateStationCommandValidator : AbstractValidator<CreateStationCommand>
{
    public CreateStationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("İstasyon adı boş olamaz.")
            .MaximumLength(200).WithMessage("İstasyon adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adres boş olamaz.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Şehir boş olamaz.");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("İlçe boş olamaz.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(35.0, 43.0)
            .WithMessage("Enlem değeri Türkiye sınırları içinde olmalıdır (35-43).");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(25.0, 45.0)
            .WithMessage("Boylam değeri Türkiye sınırları içinde olmalıdır (25-45).");

        RuleFor(x => x.OperatorName)
            .NotEmpty().WithMessage("Operatör adı boş olamaz.");

        RuleFor(x => x.Connectors)
            .NotEmpty().WithMessage("En az bir şarj soketi eklenmelidir.");

        RuleForEach(x => x.Connectors).ChildRules(connector =>
        {
            connector.RuleFor(c => c.PowerKW)
                .GreaterThan(0).WithMessage("Şarj gücü 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(500).WithMessage("Şarj gücü 500 kW'ı aşamaz.");

            connector.RuleFor(c => c.PricePerKWh)
                .GreaterThanOrEqualTo(0).When(c => c.PricePerKWh.HasValue)
                .WithMessage("Fiyat negatif olamaz.");
        });
    }
}
