namespace Item.Api.Data;

public class PurchasedProduct : BaseEntity
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double? PriceAtPurchase { get; set; }

    public string OwnerId { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; }

    public virtual Product Product { get; set; }
}
