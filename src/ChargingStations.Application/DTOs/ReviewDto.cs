namespace ChargingStations.Application.DTOs;

/// <summary>
/// Yorum bilgisi — istasyon detayında ve kullanıcı profilinde gösterilecek.
/// </summary>
public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public Guid StationId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
