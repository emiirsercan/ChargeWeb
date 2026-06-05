using ChargingStations.Application;
using ChargingStations.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ── Servis Kayıtları ────────────────────────────────────────────────
// Her katman kendi servislerini kendisi kaydeder (ServiceRegistration.cs)

// Application katmanı: MediatR, FluentValidation
builder.Services.AddApplicationServices();

// Persistence katmanı: DbContext, Repository'ler
builder.Services.AddPersistenceServices(builder.Configuration);

// ── Temel API Servisleri ─────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── CORS (React frontend için) ───────────────────────────────────────
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

// ── Middleware Pipeline ──────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
