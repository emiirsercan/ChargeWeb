using ChargingStations.Domain.Entities;

namespace ChargingStations.Application.Interfaces.Repositories;

/// <summary>
/// Kullanıcı repository interface'i.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}
