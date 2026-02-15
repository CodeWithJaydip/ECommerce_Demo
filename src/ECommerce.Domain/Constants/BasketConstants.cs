namespace ECommerce.Domain.Constants;

public static class BasketConstants
{
    public const int MaxQuantityPerItem = 99;
    public const int MinQuantity = 1;

    public const string BasketNotFound = "Basket not found";
    public const string BasketItemNotFound = "Item not found in basket";
    public const string BasketEmpty = "Basket is empty";
    public const string ProductNotFound = "Product not found";
    public const string ProductNotAvailable = "Product is not available";
    public const string InsufficientStock = "Requested quantity exceeds available stock";
    public const string QuantityInvalid = "Quantity must be between 1 and 99";
    public const string ItemAddedToBasket = "Item added to basket";
    public const string ItemUpdated = "Basket item updated";
    public const string ItemRemoved = "Item removed from basket";
    public const string BasketCleared = "Basket cleared";
}
