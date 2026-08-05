namespace Item.Api.DTO;

public class StockCreateDTO
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}
