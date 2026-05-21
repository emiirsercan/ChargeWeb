namespace ChargingStations.Shared.Exceptions;

/// <summary>
/// Yetkilendirme hatası — kullanıcının bu işlemi yapmaya yetkisi olmadığında.
/// ExceptionHandlingMiddleware bunu 403 Forbidden olarak döner.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message)
    {
    }
}
