using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Features.ShippingAddress.Interfaces;
using ECommerce.Infrastructure.Data;
using ShippingAddressEntity = ECommerce.Domain.Entities.ShippingAddress;

namespace ECommerce.Infrastructure.Repositories;

public class ShippingAddressRepository(ApplicationDbContext context) : IShippingAddressRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<ShippingAddressEntity>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.ShippingAddresses
            .AsNoTracking()
            .Where(sa => sa.UserId == userId && sa.IsActive)
            .OrderByDescending(sa => sa.IsDefault)
            .ThenByDescending(sa => sa.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShippingAddressEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ShippingAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(sa => sa.Id == id && sa.IsActive, cancellationToken);
    }

    public async Task<ShippingAddressEntity?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ShippingAddresses
            .FirstOrDefaultAsync(sa => sa.Id == id && sa.IsActive, cancellationToken);
    }

    public async Task<ShippingAddressEntity> CreateAsync(ShippingAddressEntity address, CancellationToken cancellationToken = default)
    {
        address.CreatedAt = DateTime.UtcNow;
        address.IsActive = true;
        await _context.ShippingAddresses.AddAsync(address, cancellationToken);
        return address;
    }

    public async Task ClearDefaultAsync(int userId, CancellationToken cancellationToken = default)
    {
        var defaults = await _context.ShippingAddresses
            .Where(sa => sa.UserId == userId && sa.IsDefault && sa.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var address in defaults)
        {
            address.IsDefault = false;
            address.UpdatedAt = DateTime.UtcNow;
        }
    }
}
