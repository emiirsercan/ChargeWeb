using ChargingStations.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChargingStations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IStationSyncService _syncService;

    public AdminController(IStationSyncService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>
    /// OpenChargeMap'ten istasyonları çeker ve DB'ye kaydeder.
    /// İlk kurulumda veya güncelleme gerektiğinde kullanılır.
    /// </summary>
    [HttpPost("sync-stations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncStations([FromQuery] string? countryCode = "TR", [FromQuery] int maxResults = 50000)
    {
        var count = await _syncService.SyncStationsAsync(countryCode, maxResults);
        return Ok(new { message = $"Sync tamamlandı: {count} istasyon işlendi." });
    }

    [HttpGet("temp-users")]
    public async Task<IActionResult> GetTempUsers([FromServices] ChargingStations.Persistence.Context.AppDbContext context)
    {
        var users = await context.Users
            .Select(u => new { u.Email, u.FullName, u.Role })
            .ToListAsync();
        return Ok(users);
    }
}
