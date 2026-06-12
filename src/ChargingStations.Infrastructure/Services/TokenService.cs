using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ChargingStations.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ChargingStations.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(Guid userId, string email, string role)
    {
        // ── 1. Gizli anahtarı oku (appsettings.json → Jwt:Key) ──
        // Bu anahtar sadece sunucuda var. Kimse bilmez.
        // Token'ı imzalamak için kullanılır.
        var secretKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured in appsettings.json");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        // ── 2. İmza algoritmasını belirle ──
        // HMAC-SHA256: Hem hızlı hem güvenli
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // ── 3. Token'ın içine yazılacak bilgileri hazırla (Claims) ──
        // Claims = "İddialar" → "Ben şu kişiyim, şu yetkiye sahibim"
        // Bouncer bilekliğe bunları yazıyor
        var claims = new[]
        {
            // Sub (Subject): Token kime ait? → UserId
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),

            // Email: Kullanıcının email adresi
            new Claim(JwtRegisteredClaimNames.Email, email),

            // Role: Kullanıcının rolü (Admin/User)
            // [Authorize(Roles = "Admin")] bu claim'e bakar
            new Claim(ClaimTypes.Role, role),

            // Jti (JWT ID): Her token'a benzersiz bir ID
            // Aynı token'ın tekrar kullanılmasını önler
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Iat (Issued At): Token ne zaman üretildi?
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        // ── 4. Token süresini belirle ──
        var expirationHours = double.Parse(
            _configuration["Jwt:ExpirationHours"] ?? "1"
        );

        // ── 5. Token'ı oluştur ──
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],       // Kim verdi: "ChargeWebAPI"
            audience: _configuration["Jwt:Audience"],   // Kime verildi: "ChargeWebClient"
            claims: claims,                              // İçindeki bilgiler
            expires: DateTime.UtcNow.AddHours(expirationHours), // Ne zaman sona erer
            signingCredentials: credentials              // İmza
        );

        // ── 6. Token'ı string'e çevir ve döndür ──
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        // 32 byte rastgele veri üret ve Base64'e çevir
        // Bu token'ın içinde hiçbir bilgi yok — sadece rastgele bir string
        // Sunucu tarafında veritabanında saklanır ve eşleştirilir
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
