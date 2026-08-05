namespace Item.Api.DTO;

public class StockEditDTO
{
    public int StockId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}
