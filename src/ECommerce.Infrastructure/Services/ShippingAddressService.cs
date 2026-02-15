using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Application.Features.ShippingAddress.DTOs.Requests;
using ECommerce.Application.Features.ShippingAddress.DTOs.Responses;
using ECommerce.Application.Features.ShippingAddress.Interfaces;
using ECommerce.Domain.Constants;
using ShippingAddressEntity = ECommerce.Domain.Entities.ShippingAddress;

namespace ECommerce.Infrastructure.Services;

public class ShippingAddressService(
    IShippingAddressRepository shippingAddressRepository,
    IUnitOfWork unitOfWork) : IShippingAddressService
{
    private readonly IShippingAddressRepository _shippingAddressRepository = shippingAddressRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<ShippingAddressResponse>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var addresses = await _shippingAddressRepository.GetByUserIdAsync(userId, cancellationToken);
        return addresses.Select(MapToResponse).ToList();
    }

    public async Task<ShippingAddressResponse> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var address = await _shippingAddressRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(ShippingAddressConstants.AddressNotFound);

        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException(ShippingAddressConstants.UnauthorizedAccess);
        }

        return MapToResponse(address);
    }

    public async Task<ShippingAddressResponse> CreateAsync(int userId, CreateShippingAddressRequest request, CancellationToken cancellationToken = default)
    {
        // If setting as default, clear existing defaults
        if (request.IsDefault)
        {
            await _shippingAddressRepository.ClearDefaultAsync(userId, cancellationToken);
        }

        var address = new ShippingAddressEntity
        {
            UserId = userId,
            FullName = request.FullName.Trim(),
            AddressLine1 = request.AddressLine1.Trim(),
            AddressLine2 = request.AddressLine2?.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Country = request.Country.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            IsDefault = request.IsDefault
        };

        address = await _shippingAddressRepository.CreateAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(address);
    }

    public async Task<ShippingAddressResponse> UpdateAsync(int id, int userId, UpdateShippingAddressRequest request, CancellationToken cancellationToken = default)
    {
        var address = await _shippingAddressRepository.GetByIdForUpdateAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(ShippingAddressConstants.AddressNotFound);

        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException(ShippingAddressConstants.UnauthorizedAccess);
        }

        // If setting as default, clear existing defaults
        if (request.IsDefault && !address.IsDefault)
        {
            await _shippingAddressRepository.ClearDefaultAsync(userId, cancellationToken);
        }

        address.FullName = request.FullName.Trim();
        address.AddressLine1 = request.AddressLine1.Trim();
        address.AddressLine2 = request.AddressLine2?.Trim();
        address.City = request.City.Trim();
        address.State = request.State.Trim();
        address.PostalCode = request.PostalCode.Trim();
        address.Country = request.Country.Trim();
        address.PhoneNumber = request.PhoneNumber?.Trim();
        address.IsDefault = request.IsDefault;
        address.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(address);
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var address = await _shippingAddressRepository.GetByIdForUpdateAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(ShippingAddressConstants.AddressNotFound);

        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException(ShippingAddressConstants.UnauthorizedAccess);
        }

        address.IsActive = false;
        address.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ShippingAddressResponse MapToResponse(ShippingAddressEntity address)
    {
        return new ShippingAddressResponse
        {
            Id = address.Id,
            UserId = address.UserId,
            FullName = address.FullName,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            PhoneNumber = address.PhoneNumber,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt,
            UpdatedAt = address.UpdatedAt
        };
    }
}
