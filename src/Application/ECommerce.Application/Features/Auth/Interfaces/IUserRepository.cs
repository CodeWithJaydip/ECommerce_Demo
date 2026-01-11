using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Auth.Interfaces;

/// <summary>
/// Repository interface for User entity
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<Role>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);
}
