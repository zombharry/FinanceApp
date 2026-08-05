namespace Item.Api.DTO;

public class DeliveryEditDTO
{
    public string DeliveryId { get; set; }

    public Guid DeliveredItemId { get; set; }

    public int Amount { get; set; }

    public double Cost { get; set; }
}
