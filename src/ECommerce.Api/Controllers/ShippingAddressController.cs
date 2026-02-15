using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.ShippingAddress.DTOs.Requests;
using ECommerce.Application.Features.ShippingAddress.DTOs.Responses;
using ECommerce.Application.Features.ShippingAddress.Interfaces;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShippingAddressController(IShippingAddressService shippingAddressService) : ControllerBase
{
    private readonly IShippingAddressService _shippingAddressService = shippingAddressService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ShippingAddressResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var addresses = await _shippingAddressService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(ApiResponse<List<ShippingAddressResponse>>.SuccessResponse(addresses, "Addresses retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ShippingAddressResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var address = await _shippingAddressService.GetByIdAsync(id, userId, cancellationToken);
        return Ok(ApiResponse<ShippingAddressResponse>.SuccessResponse(address, "Address retrieved successfully"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ShippingAddressResponse>>> Create(
        [FromBody] CreateShippingAddressRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var address = await _shippingAddressService.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = address.Id },
            ApiResponse<ShippingAddressResponse>.SuccessResponse(address, "Address created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ShippingAddressResponse>>> Update(
        int id, [FromBody] UpdateShippingAddressRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var address = await _shippingAddressService.UpdateAsync(id, userId, request, cancellationToken);
        return Ok(ApiResponse<ShippingAddressResponse>.SuccessResponse(address, "Address updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _shippingAddressService.DeleteAsync(id, userId, cancellationToken);
        return Ok(ApiResponse.SuccessResponse("Address deleted successfully"));
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("User not authenticated");
    }
}
