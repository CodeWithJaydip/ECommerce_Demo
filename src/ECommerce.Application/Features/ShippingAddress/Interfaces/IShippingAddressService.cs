using ECommerce.Application.Features.ShippingAddress.DTOs.Requests;
using ECommerce.Application.Features.ShippingAddress.DTOs.Responses;

namespace ECommerce.Application.Features.ShippingAddress.Interfaces;

public interface IShippingAddressService
{
    Task<List<ShippingAddressResponse>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<ShippingAddressResponse> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<ShippingAddressResponse> CreateAsync(int userId, CreateShippingAddressRequest request, CancellationToken cancellationToken = default);
    Task<ShippingAddressResponse> UpdateAsync(int id, int userId, UpdateShippingAddressRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}
