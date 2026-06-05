namespace ChargingStations.Application.DTOs;

/// <summary>
/// Sayfalanmış sonuç wrapper'ı.
/// API'den dönen listelerde pagination meta bilgisi taşır.
/// 
/// Frontend bunu kullanarak "Sayfa 2/10" gibi bilgiler gösterebilir.
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
