using System.Text;
using ChargingStations.API.Middlewares;
using ChargingStations.Application;
using ChargingStations.Infrastructure;
using ChargingStations.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════════════════════════════
// ── SERVİS KAYITLARI ────────────────────────────────────────────
// Her katman kendi servislerini kendisi kaydeder (ServiceRegistration.cs)
// Program.cs sadece "binaya hangi sistemler bağlanacak" der.
//
// Senaryo: "Binanın açılış günü"
//   Mimar (Program.cs) tüm taşeronları çağırıyor:
//   - "Elektrik ekibi, kablolarınızı çekin!" → Application
//   - "Depo ekibi, raflarınızı kurun!" → Persistence
//   - "Güvenlik ekibi, kameralarınızı takın!" → Infrastructure
// ══════════════════════════════════════════════════════════════════

// Application katmanı: MediatR (garsonlar), FluentValidation (kalite kontrol)
builder.Services.AddApplicationServices();

// Persistence katmanı: DbContext (veritabanı bağlantısı), Repository'ler (depo)
builder.Services.AddPersistenceServices(builder.Configuration);

// Infrastructure katmanı: TokenService (bileklik makinesi), CacheService (not defteri)
builder.Services.AddInfrastructureServices();

// ══════════════════════════════════════════════════════════════════
// ── JWT AUTHENTICATION ──────────────────────────────────────────
// "Binanın güvenlik kontrol noktası"
//
// Her gelen isteğin Header'ında JWT token var mı?
//   Authorization: Bearer eyJhbGciOiJIUzI1NiJ9...
//
// Varsa: Token'ı doğrula → geçerli mi? → süre dolmuş mu? → imza doğru mu?
// Yoksa: [Authorize] işaretli endpoint'lere erişimi engelle
// ══════════════════════════════════════════════════════════════════
builder.Services.AddAuthentication(options =>
{
    // Default şema: JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT Key is not configured!");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        // ── Token'ı kim üretmiş? ──
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // "ChargeWebAPI"

        // ── Token kime verilmiş? ──
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],  // "ChargeWebClient"

        // ── Token imzası doğru mu? ──
        // Bu en kritik kontrol: Token'ın değiştirilmediğini garantiler
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        // ── Token süresi dolmuş mu? ──
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero  // Varsayılan 5 dakika tolerans → 0'a çek (kesin süre)
    };
});

// ── Temel API Servisleri ─────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ══════════════════════════════════════════════════════════════════
// ── SWAGGER + JWT ────────────────────────────────────────────────
// Swagger'da "Authorize" butonu ekle
// Test ederken token'ı buraya yapıştırabilelim
// ══════════════════════════════════════════════════════════════════
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ChargeWeb API",
        Version = "v1",
        Description = "EV Charging Stations API — Elektrikli araç şarj istasyonları"
    });

    // JWT Bearer token giriş alanı ekle
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ı girin. Örnek: eyJhbGciOiJIUzI1NiJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── CORS (React frontend için) ───────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite default portu
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ══════════════════════════════════════════════════════════════════
// ── MIDDLEWARE PIPELINE ─────────────────────────────────────────
// İsteklerin geçtiği "koridor" — sıralama ÇOK önemli!
//
// Request → [Exception] → [CORS] → [Auth] → [Authorize] → [Controller]
//
// Neden bu sıra?
//   1. ExceptionMiddleware en dışta: Her hatayı yakalar
//   2. CORS: Tarayıcı güvenlik kontrolü
//   3. Authentication: "Kim bu kişi?" (token doğrulama)
//   4. Authorization: "Bu kişi bunu yapabilir mi?" (rol kontrolü)
//   5. Controller: İsteği işle
// ══════════════════════════════════════════════════════════════════

// Hata yönetim müdürü — en dışta olmalı (tüm hataları yakalar)
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

// ── Authentication → Authorization sırası KRİTİK! ──
// Önce "Sen kimsin?" sonra "Ne yapabilirsin?"
// Bu sıra değişirse [Authorize] çalışmaz!
app.UseAuthentication();  // ← "Bilekliği kontrol et"
app.UseAuthorization();   // ← "VIP alanına girebilir mi?"

app.MapControllers();

app.Run();
