using ChargingStations.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
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
    public async Task<IActionResult> SyncStations([FromQuery] int maxResults = 500)
    {
        var count = await _syncService.SyncStationsAsync("TR", maxResults);
        return Ok(new { message = $"Sync tamamlandı: {count} istasyon işlendi." });
    }
}
