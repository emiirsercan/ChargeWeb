using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ChargingStations.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Normal akış: İsteği bir sonraki middleware'e / controller'a ilet
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen hata: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }


    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            // ── 401 Unauthorized ──
            // Senaryo: Yanlış şifre ile login denendi
            UnauthorizedAccessException ex =>
                (HttpStatusCode.Unauthorized, ex.Message, (object?)null),

            // ── 400 Bad Request (Validation hataları) ──
            // Senaryo: Email boş gönderildi, şifre 3 karakter
            // FluentValidation handler'da hata fırlatır
            ValidationException ex =>
                (HttpStatusCode.BadRequest, "Doğrulama hatası",
                    (object?)ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })),

            // ── 404 Not Found ──
            // Senaryo: Var olmayan bir istasyon ID'si ile sorgu yapıldı
            KeyNotFoundException ex =>
                (HttpStatusCode.NotFound, ex.Message, (object?)null),

            // ── 400 Bad Request (Genel) ──
            // Senaryo: Geçersiz formatta veri gönderildi
            ArgumentException ex =>
                (HttpStatusCode.BadRequest, ex.Message, (object?)null),

            // ── 409 Conflict ──
            // Senaryo: Zaten kayıtlı bir email ile register denendi
            InvalidOperationException ex =>
                (HttpStatusCode.Conflict, ex.Message, (object?)null),

            // ── 500 Internal Server Error ──
            // Senaryo: Beklenmeyen bir hata (veritabanı bağlantısı koptu vs.)
            _ => (HttpStatusCode.InternalServerError,
                  exception.InnerException?.InnerException?.Message 
                      ?? exception.InnerException?.Message 
                      ?? exception.Message,
                  (object?)exception.StackTrace)
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            message,
            errors
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsJsonAsync(response, jsonOptions);
    }
}
