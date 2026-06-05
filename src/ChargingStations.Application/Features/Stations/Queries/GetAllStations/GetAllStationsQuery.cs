using ChargingStations.Application.DTOs;
using MediatR;

namespace ChargingStations.Application.Features.Stations.Queries.GetAllStations;

/// <summary>
/// "Tüm istasyonları getir" sorgusu.
/// 
/// MediatR ile CQRS nasıl çalışır?
/// 1. Controller bu Query nesnesini oluşturur
/// 2. MediatR, bu Query'ye karşılık gelen Handler'ı bulur
/// 3. Handler işi yapar ve sonucu döner
/// 4. Controller sonucu HTTP response olarak gönderir
/// 
/// Controller → MediatR → Handler → Repository → DB
/// </summary>
public class GetAllStationsQuery : IRequest<PagedResult<StationDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
