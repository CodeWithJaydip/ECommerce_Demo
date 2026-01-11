using ECommerce.Application.Features.Auth.DTOs;
using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Services;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Service implementation for authentication
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create user
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Save user first to get the ID
        user = await _userRepository.CreateAsync(user, cancellationToken);

        // Get Customer role or create it
        var customerRole = await _roleRepository.GetByNameAsync(nameof(ECommerce.Domain.Enums.UserRole.Customer), cancellationToken);
        if (customerRole == null)
        {
            customerRole = new Role
            {
                Name = nameof(ECommerce.Domain.Enums.UserRole.Customer),
                Description = "Customer role",
                IsActive = true
            };
            customerRole = await _roleRepository.CreateAsync(customerRole, cancellationToken);
        }

        // Assign Customer role to user
        await _userRepository.AddUserRoleAsync(user.Id, customerRole.Id, cancellationToken);

        // Get user roles
        var roles = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);
        var roleNames = roles.Select(r => r.Name).ToList();

        // Generate token
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, roleNames);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roleNames
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if account is locked
        if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Account is locked. Please try again later");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            // Increment login attempt count
            user.LoginAttemptCount++;
            if (user.LoginAttemptCount >= 5)
            {
                user.LockedUntil = DateTime.UtcNow.AddMinutes(30); // Lock for 30 minutes
            }
            await _userRepository.UpdateAsync(user, cancellationToken);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Reset login attempt count and update last login
        user.LoginAttemptCount = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Get user roles
        var roles = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);
        var roleNames = roles.Select(r => r.Name).ToList();

        // Generate token
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, roleNames);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roleNames
            }
        };
    }

    public Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var principal = _jwtTokenService.ValidateToken(token);
        return Task.FromResult(principal != null);
    }
}
