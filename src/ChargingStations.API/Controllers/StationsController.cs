using ChargingStations.Application.DTOs;
using ChargingStations.Application.Features.Stations.Commands.CreateStation;
using ChargingStations.Application.Features.Stations.Queries.GetAllStations;
using ChargingStations.Application.Features.Stations.Queries.GetNearbyStations;
using ChargingStations.Application.Features.Stations.Queries.GetStationById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChargingStations.API.Controllers;

/// <summary>
/// Stations Controller — "Şarj İstasyonları Garson Ekibi"
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Bir harita uygulaması düşün (Google Maps gibi):
///   - Kullanıcı haritayı açtı → Yakınındaki şarj istasyonlarını gördü
///   - Bir istasyona tıkladı → Detaylarını gördü (connector tipleri, yorumlar)
///   - Admin yeni istasyon ekledi → Haritada belirdi
///
/// ─── ENDPOINT'LER ──────────────────────────────────────────
/// GET  /api/stations              → Tüm istasyonları listele (sayfalı)
/// GET  /api/stations/{id}         → Tek istasyonun detayı
/// GET  /api/stations/nearby       → Yakındaki istasyonlar (Haversine)
/// POST /api/stations              → Yeni istasyon ekle (sadece Admin!)
///
/// ─── YETKİ SEVİYELERİ ─────────────────────────────────────
/// [AllowAnonymous] → Herkes erişebilir (login gerekmez)
///   → GET istasyonları: Kullanıcı uygulamayı açtığında hemen görsün
///
/// [Authorize(Roles = "Admin")] → Sadece Admin rolüne sahip kullanıcılar
///   → POST yeni istasyon: Rastgele biri istasyon ekleyemesin
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm istasyonları sayfalı olarak listeler.
    ///
    /// GET /api/stations?page=1&amp;pageSize=10
    ///
    /// Senaryo:
    ///   React harita sayfası yüklendi
    ///       ↓
    ///   fetch("/api/stations?page=1&amp;pageSize=10")
    ///       ↓
    ///   StationsController.GetAll()
    ///       ↓
    ///   MediatR → GetAllStationsQueryHandler
    ///       ↓
    ///   StationRepository.GetAllAsync(page, pageSize)
    ///       ↓
    ///   Supabase → SELECT * FROM stations LIMIT 10 OFFSET 0
    ///       ↓
    ///   Response: { items: [...], totalCount: 500, page: 1, pageSize: 10 }
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<StationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllStationsQuery { Page = page, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Tek bir istasyonun detayını döner (connector'lar + yorumlar dahil).
    ///
    /// GET /api/stations/{id}
    ///
    /// Senaryo:
    ///   Kullanıcı haritada bir istasyona tıkladı
    ///       ↓
    ///   fetch("/api/stations/abc-123-def")
    ///       ↓
    ///   StationsController.GetById()
    ///       ↓
    ///   Handler → Repository → Include(Connectors).Include(Reviews)
    ///       ↓
    ///   Response: {
    ///     name: "Togg Şarj İstasyonu",
    ///     connectors: [{ type: "CCS2", power: 150 }],
    ///     reviews: [{ rating: 5, comment: "Harika!" }]
    ///   }
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StationDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetStationByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının konumuna yakın istasyonları döner (Haversine formülü).
    ///
    /// GET /api/stations/nearby?latitude=41.01&amp;longitude=28.97&amp;radiusKm=5&amp;page=1&amp;pageSize=10
    ///
    /// Senaryo:
    ///   Kullanıcı telefonda "Yakınımdaki İstasyonlar" butonuna tıkladı
    ///       ↓
    ///   navigator.geolocation.getCurrentPosition() → { lat: 41.01, lng: 28.97 }
    ///       ↓
    ///   fetch("/api/stations/nearby?latitude=41.01&amp;longitude=28.97&amp;radiusKm=5")
    ///       ↓
    ///   Handler → StationRepository.GetNearbyAsync(lat, lng, radius)
    ///       ↓
    ///   Haversine formülü ile mesafe hesaplanır:
    ///     d = 2R × arcsin(√(sin²(Δφ/2) + cos(φ1)·cos(φ2)·sin²(Δλ/2)))
    ///       ↓
    ///   5 km yarıçapındaki istasyonlar → mesafeye göre sıralı döner
    /// </summary>
    [HttpGet("nearby")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<StationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetNearbyStationsQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            Page = page,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Yeni şarj istasyonu ekler (sadece Admin yetkisi gerekli).
    ///
    /// POST /api/stations
    /// Header: Authorization: Bearer eyJ...
    ///
    /// Senaryo:
    ///   Admin panelinden yeni istasyon formu dolduruldu:
    ///   {
    ///     "name": "Togg Şarj İstasyonu",
    ///     "latitude": 41.0082,
    ///     "longitude": 28.9784,
    ///     "address": "İstanbul, Sultanahmet",
    ///     "city": "İstanbul",
    ///     "connectors": [
    ///       { "type": "CCS2", "power": 150, "status": "Available" }
    ///     ]
    ///   }
    ///       ↓
    ///   JWT Middleware → Token geçerli mi? ✓
    ///   Authorization  → Admin rolü var mı? ✓
    ///       ↓
    ///   Handler → Station entity oluşturuldu → DB'ye kaydedildi
    ///       ↓
    ///   Cache invalidation: "stations" prefix'li tüm cache temizlendi
    ///       ↓
    ///   Response 201: { id: "new-guid", name: "Togg Şarj İstasyonu" }
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(StationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateStationCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
