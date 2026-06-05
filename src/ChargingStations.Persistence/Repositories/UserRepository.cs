using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using ChargingStations.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChargingStations.Persistence.Repositories;

/// <summary>
/// IUserRepository'nin EF Core implementasyonu.
///
/// ─── SENARYO ───────────────────────────────────────────────
/// Kullanıcı "emre@example.com" ile giriş yapıyor.
/// 
/// LoginCommandHandler şunu yapar:
///   1. _userRepository.GetByEmailAsync("emre@example.com")
///   2. Dönen User null ise → "Kullanıcı bulunamadı"
///   3. Null değilse → BCrypt.Verify(password, user.PasswordHash)
///   4. Doğruysa → JWT token oluştur
///
/// Bu akışta UserRepository, Handler'ın elini kirletmeden
/// "email ile veritabanında bul" işini yapar.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Email ile kullanıcı arar.
    /// Login ve "email zaten kayıtlı mı?" kontrolü için kullanılır.
    ///
    /// StringComparison.OrdinalIgnoreCase neden?
    /// → "Emre@example.com" ile "emre@example.com" aynı kullanıcı
    /// → Veritabanındaki email unique index'i de bunu destekler
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        return user;
    }

    public Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Email'in veritabanında kayıtlı olup olmadığını hızlıca kontrol eder.
    ///
    /// AnyAsync() neden CountAsync() > 0 yerine tercih edilir?
    /// → AnyAsync() ilk eşleşen kaydı bulunca SQL EXISTS ile durur
    /// → CountAsync() tüm eşleşen kayıtları sayar
    /// → Email unique olduğu için sonuç aynı ama AnyAsync() daha hızlı
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
