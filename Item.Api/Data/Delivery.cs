namespace Item.Api.Data;

public class Delivery : BaseEntity
{
    public Guid DeliveryId { get; set; }

    public Guid DeliveredItemId { get; set; }

    public int Amount { get; set; }

    public double Cost { get; set; }

    public virtual Product DeliveredItem { get; set; }
}