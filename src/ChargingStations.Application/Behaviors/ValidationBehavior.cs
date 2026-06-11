using FluentValidation;
using MediatR;

namespace ChargingStations.Application.Behaviors;

/// <summary>
/// Validation Pipeline Behavior — "Restoranın Kalite Kontrol Şefi"
///
/// ─── PROBLEM ───────────────────────────────────────────────
/// Validator'ları yazdık (RegisterCommandValidator gibi) ama
/// MediatR bunları otomatik çalıştırmıyor!
///
///   Request → Handler (validator hiç çağrılmadı!)
///
/// ─── ÇÖZÜM ─────────────────────────────────────────────────
/// Bu sınıf MediatR pipeline'ına "araya giren" bir davranış ekler:
///
///   Request → [ValidationBehavior] → Handler
///                    ↓
///             Validator var mı?
///             Evet → Kuralları kontrol et
///                    Hata varsa → ValidationException fırlat (400)
///                    Hata yoksa → Handler'a devam et
///             Hayır → Direkt Handler'a git
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Kullanıcı register oluyor:
///   { email: "", password: "12", fullName: "" }
///
/// ValidationBehavior devreye girer:
///   1. RegisterCommandValidator'ı bulur
///   2. Kuralları çalıştırır:
///      - email boş → HATA
///      - password 6 karakterden kısa → HATA
///      - fullName boş → HATA
///   3. ValidationException fırlatır
///   4. GlobalExceptionMiddleware yakalar → 400 Bad Request:
///      {
///        "status": 400,
///        "message": "Doğrulama hatası",
///        "errors": [
///          { "propertyName": "Email", "errorMessage": "Email boş olamaz." },
///          { "propertyName": "Password", "errorMessage": "Şifre en az 6 karakter olmalıdır." }
///        ]
///      }
///   5. Handler hiç çalışmadı! → DB'ye bozuk veri gitmedi!
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // ── 1. Bu request için validator var mı? ──
        if (!_validators.Any())
        {
            // Validator yoksa direkt handler'a git
            // Örnek: GetAllStationsQuery → validator'ı yok → direkt geç
            return await next();
        }

        // ── 2. Tüm validator'ları çalıştır ──
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        // ── 3. Hataları topla ──
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        // ── 4. Hata varsa exception fırlat ──
        if (failures.Count > 0)
        {
            // Bu exception GlobalExceptionMiddleware tarafından yakalanır
            // → 400 Bad Request + hata detayları
            throw new ValidationException(failures);
        }

        // ── 5. Hata yoksa handler'a devam et ──
        return await next();
    }
}
