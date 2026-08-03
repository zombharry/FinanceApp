using System.ComponentModel.DataAnnotations;

namespace Item.Api.Data;

public class Stock : BaseEntity
{
    public int StockId { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }

    public int Quantity { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } 
}
