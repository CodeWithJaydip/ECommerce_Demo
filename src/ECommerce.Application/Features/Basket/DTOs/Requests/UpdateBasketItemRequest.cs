using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Constants;

namespace ECommerce.Application.Features.Basket.DTOs.Requests;

public record UpdateBasketItemRequest
{
    [Required(ErrorMessage = BasketConstants.QuantityInvalid)]
    [Range(BasketConstants.MinQuantity, BasketConstants.MaxQuantityPerItem, ErrorMessage = BasketConstants.QuantityInvalid)]
    public int Quantity { get; init; }
}
