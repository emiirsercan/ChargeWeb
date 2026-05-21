namespace ChargingStations.Shared.Exceptions;

/// <summary>
/// Doğrulama hatası — FluentValidation tarafından yakalanan hatalar için.
/// ExceptionHandlingMiddleware bunu 400 Bad Request olarak döner.
/// </summary>
public class CustomValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public CustomValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
