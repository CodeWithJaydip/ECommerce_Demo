using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Auth.Interfaces;

/// <summary>
/// Repository interface for Role entity
/// </summary>
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role> CreateAsync(Role role, CancellationToken cancellationToken = default);
    Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default);
}
