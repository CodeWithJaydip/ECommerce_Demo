using ECommerce.Application.Features.Auth.DTOs.Requests;
using ECommerce.Application.Features.Auth.DTOs.Responses;

namespace ECommerce.Application.Features.Auth.Interfaces;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
