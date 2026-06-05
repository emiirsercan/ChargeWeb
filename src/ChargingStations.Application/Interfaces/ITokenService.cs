namespace ChargingStations.Application.Interfaces;

/// <summary>
/// JWT token oluşturma servisi interface'i.
/// Login başarılı olduğunda access token + refresh token üretir.
/// </summary>
public interface ITokenService
{
    /// <summary>Kullanıcı bilgilerinden JWT access token oluşturur</summary>
    string GenerateAccessToken(Guid userId, string email, string role);

    /// <summary>Refresh token oluşturur (opaque string)</summary>
    string GenerateRefreshToken();
}
