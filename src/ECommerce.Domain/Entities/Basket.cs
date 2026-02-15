namespace ECommerce.Domain.Entities;

public class Basket : BaseEntity
{
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<BasketItem> Items { get; set; } = [];
}
