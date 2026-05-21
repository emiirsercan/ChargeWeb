namespace ChargingStations.Shared.Exceptions;

/// <summary>
/// Kaynak bulunamadığında fırlatılan exception.
/// Controller'daki ExceptionHandlingMiddleware bunu yakalayıp 404 döner.
/// 
/// Kullanım:
///   throw new NotFoundException("ChargingStation", stationId);
///   → "ChargingStation with id '...' was not found."
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.")
    {
    }
}
