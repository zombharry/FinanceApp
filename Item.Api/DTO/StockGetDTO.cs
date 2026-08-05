namespace Item.Api.DTO;

public class StockGetDTO
{
    public int StockId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}
