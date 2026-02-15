using ShippingAddressEntity = ECommerce.Domain.Entities.ShippingAddress;

namespace ECommerce.Application.Features.ShippingAddress.Interfaces;

public interface IShippingAddressRepository
{
    Task<List<ShippingAddressEntity>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<ShippingAddressEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ShippingAddressEntity?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<ShippingAddressEntity> CreateAsync(ShippingAddressEntity address, CancellationToken cancellationToken = default);
    Task ClearDefaultAsync(int userId, CancellationToken cancellationToken = default);
}
