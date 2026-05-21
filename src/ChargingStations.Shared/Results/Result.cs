namespace ChargingStations.Shared.Results;

/// <summary>
/// Operasyon sonucunu temsil eden wrapper sınıf (veri taşımadan).
/// 
/// NEDEN RESULT PATTERN?
/// ─────────────────────
/// Geleneksel yaklaşım: Exception fırlat, Controller'da yakala.
/// Sorun: Exception'lar pahalıdır (stack trace oluşturur), kontrol akışı için kullanılmamalıdır.
/// 
/// Result pattern ile:
/// - Her metod ya başarılı ya da başarısız bir Result döner
/// - Exception sadece gerçekten beklenmeyen durumlar için fırlatılır
/// - Controller, Result'a bakarak HTTP status code belirler
/// 
/// Kullanım:
///   return Result.Success();                    → 200 OK
///   return Result.Failure("Station not found"); → 404 veya 400
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

/// <summary>
/// Generic Result — veri taşıyan versiyon.
/// 
/// Kullanım:
///   return Result{StationDto}.Success(stationDto);       → 200 OK + data
///   return Result{StationDto}.Failure("Not found");      → 404, data yok
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool isSuccess, T? data, string? error)
        : base(isSuccess, error)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public new static Result<T> Failure(string error) => new(false, default, error);
}
