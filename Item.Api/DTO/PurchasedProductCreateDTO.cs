namespace Item.Api.DTO;

public class PurchasedProductCreateDTO
{
    public Guid ProductId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double? PriceAtPurchase { get; set; }

    public int Amount { get; set; }

    public string OwnerId { get; set; }

    public Guid CategoryId { get; set; }
}
