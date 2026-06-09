using ChargingStations.Application.DTOs;
using ChargingStations.Application.Features.Auth.Commands.Login;
using ChargingStations.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStations.API.Controllers;

/// <summary>
/// Auth Controller — "Binanın Giriş Kapısı / Kimlik Kontrol Masası"
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Bir devlet dairesine girmeyi düşün:
///   1. İlk defa geldin → "Kayıt olun" masasına git (Register)
///   2. Kaydın var → "Giriş" masasına git, kimliğini göster (Login)
///   3. Kimlik doğrulandı → Sana bir ziyaretçi kartı verilir (JWT Token)
///   4. O kartla bina içinde istediğin bölüme gidebilirsin
///
/// ─── CONTROLLER'IN GÖREVİ ──────────────────────────────────
/// Controller = Garson. Asıl işi yapmaz, sadece:
///   1. Müşteriden siparişi alır (HTTP Request)
///   2. Mutfağa iletir (MediatR → Handler)
///   3. Sonucu müşteriye taşır (HTTP Response)
///
/// Hiçbir iş mantığı (business logic) burada YOKTUR!
/// Şifre kontrolü? → LoginCommandHandler'da
/// Kullanıcı oluşturma? → RegisterCommandHandler'da
/// Token üretme? → TokenService'te
///
/// Controller sadece "al → ilet → dön" yapar.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Yeni kullanıcı kaydı oluşturur.
    ///
    /// POST /api/auth/register
    ///
    /// Senaryo:
    ///   React frontend'ten form dolduruldu:
    ///   { "email": "emir@test.com", "password": "123456", "fullName": "Emir Sercan" }
    ///       ↓
    ///   AuthController.Register() çağrıldı
    ///       ↓
    ///   MediatR → RegisterCommandHandler.Handle()
    ///       ↓
    ///   1. Email zaten var mı? → Varsa 409 Conflict
    ///   2. Şifre BCrypt ile hash'lendi
    ///   3. Kullanıcı veritabanına kaydedildi
    ///   4. JWT Token üretildi
    ///       ↓
    ///   Response 201: { accessToken, refreshToken, user }
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Kullanıcı girişi yapar, JWT token döner.
    ///
    /// POST /api/auth/login
    ///
    /// Senaryo:
    ///   React frontend'ten login formu gönderildi:
    ///   { "email": "emir@test.com", "password": "123456" }
    ///       ↓
    ///   AuthController.Login() çağrıldı
    ///       ↓
    ///   MediatR → LoginCommandHandler.Handle()
    ///       ↓
    ///   1. Email ile kullanıcı bulundu
    ///   2. BCrypt.Verify(password, hash) → şifre doğrulandı
    ///   3. JWT Token üretildi
    ///       ↓
    ///   Response 200: { accessToken, refreshToken, expiresAt, user }
    ///
    ///   Frontend bu token'ı localStorage'a kaydeder:
    ///   localStorage.setItem("accessToken", response.accessToken)
    ///
    ///   Sonraki tüm isteklerde header'a ekler:
    ///   Authorization: Bearer eyJhbGciOiJIUzI1NiJ9...
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
