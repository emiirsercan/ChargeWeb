using ChargingStations.Domain.Common;
using ChargingStations.Domain.Enums;

namespace ChargingStations.Domain.Entities;

/// <summary>
/// Kullanıcı entity'si.
/// 
/// Şifre hash'lenerek saklanır (plain text asla!).
/// Her kullanıcının bir rolü vardır (User, Admin, StationOwner).
/// </summary>
public class User : BaseEntity
{
    /// <summary>Kullanıcının email adresi (unique, login için kullanılır)</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Ad soyad</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>BCrypt ile hash'lenmiş şifre</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Kullanıcı rolü</summary>
    public UserRole Role { get; set; } = UserRole.User;

    // ── Navigation Properties ──────────────────────────────
    /// <summary>Kullanıcının yazdığı yorumlar</summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>Kullanıcının favori istasyonları</summary>
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
