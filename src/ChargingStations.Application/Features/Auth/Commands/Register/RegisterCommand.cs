using ChargingStations.Application.DTOs;
using MediatR;

namespace ChargingStations.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
