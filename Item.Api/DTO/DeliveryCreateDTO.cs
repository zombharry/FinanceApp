namespace Item.Api.DTO;

public class DeliveryCreateDTO
{
    public Guid DeliveredItemId { get; set; }

    public int Amount { get; set; }

    public double Cost { get; set; }
}
