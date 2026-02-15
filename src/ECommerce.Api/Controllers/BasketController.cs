using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Basket.DTOs.Requests;
using ECommerce.Application.Features.Basket.DTOs.Responses;
using ECommerce.Application.Features.Basket.Interfaces;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasketController(IBasketService basketService) : ControllerBase
{
    private readonly IBasketService _basketService = basketService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<BasketResponse>>> GetBasket(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var basket = await _basketService.GetBasketAsync(userId, cancellationToken);
        return Ok(ApiResponse<BasketResponse>.SuccessResponse(basket, "Basket retrieved successfully"));
    }

    [HttpGet("count")]
    public async Task<ActionResult<ApiResponse<int>>> GetItemCount(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var count = await _basketService.GetItemCountAsync(userId, cancellationToken);
        return Ok(ApiResponse<int>.SuccessResponse(count, "Basket item count retrieved"));
    }

    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<BasketResponse>>> AddItem(
        [FromBody] AddBasketItemRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var basket = await _basketService.AddItemAsync(userId, request, cancellationToken);
        return Ok(ApiResponse<BasketResponse>.SuccessResponse(basket, "Item added to basket"));
    }

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<ApiResponse<BasketResponse>>> UpdateItemQuantity(
        int productId, [FromBody] UpdateBasketItemRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var basket = await _basketService.UpdateItemQuantityAsync(userId, productId, request, cancellationToken);
        return Ok(ApiResponse<BasketResponse>.SuccessResponse(basket, "Basket item updated"));
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<ApiResponse>> RemoveItem(int productId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _basketService.RemoveItemAsync(userId, productId, cancellationToken);
        return Ok(ApiResponse.SuccessResponse("Item removed from basket"));
    }

    [HttpDelete]
    public async Task<ActionResult<ApiResponse>> ClearBasket(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _basketService.ClearBasketAsync(userId, cancellationToken);
        return Ok(ApiResponse.SuccessResponse("Basket cleared"));
    }

    [HttpGet("checkout-summary")]
    public async Task<ActionResult<ApiResponse<CheckoutSummaryResponse>>> GetCheckoutSummary(
        [FromQuery] int? addressId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var summary = await _basketService.GetCheckoutSummaryAsync(userId, addressId, cancellationToken);
        return Ok(ApiResponse<CheckoutSummaryResponse>.SuccessResponse(summary, "Checkout summary retrieved"));
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
