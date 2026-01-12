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
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
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

        // Create user (tracked but not saved yet)
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
        user = await _userRepository.CreateAsync(user, cancellationToken);

        // Get Customer role or create it (tracked but not saved yet)
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
        // Note: user.Id and customerRole.Id are still 0 here, but EF Core will resolve them on SaveChanges
        // using the tracked entities and navigation properties
        await _userRepository.AddUserRoleAsync(user.Id, customerRole.Id, cancellationToken);

        // Save all changes in a single transaction
        // EF Core will resolve UserId and RoleId from navigation properties
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Refresh user to get the populated ID and roles after SaveChanges
        user = await _userRepository.GetByIdAsync(user.Id, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("Failed to create user");
        }

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
            // Save changes for failed login attempt
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Reset login attempt count and update last login
        user.LoginAttemptCount = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);
        
        // Save all changes in a single transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
