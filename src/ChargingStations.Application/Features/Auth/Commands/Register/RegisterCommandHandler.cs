using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces;
using ChargingStations.Application.Interfaces.Repositories;
using ChargingStations.Domain.Entities;
using MediatR;

namespace ChargingStations.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Email zaten kayıtlı mı?
        if (await _userRepository.EmailExistsAsync(request.Email))
            throw new InvalidOperationException("Bu email adresi zaten kayıtlı.");

        // Şifreyi hash'le (BCrypt)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = passwordHash
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Token oluştur
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString()
            }
        };
    }
}
