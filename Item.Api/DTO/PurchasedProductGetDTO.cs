namespace Item.Api.DTO;

public class PurchasedProductGetDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Guid CategoryId { get; set; }

    public double? PriceAtPurchase { get; set; }

    public string OwnerId { get; set; }
}
