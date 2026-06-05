using ChargingStations.Application.DTOs;
using ChargingStations.Application.Interfaces;
using ChargingStations.Application.Interfaces.Repositories;
using MediatR;

namespace ChargingStations.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email veya şifre hatalı.");

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
