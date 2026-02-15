namespace ECommerce.Domain.Entities;

public class BasketItem : BaseEntity
{
    public int BasketId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual Basket Basket { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
