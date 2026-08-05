using System.ComponentModel.DataAnnotations;

namespace Item.Api.Data;

public class Stock : BaseEntity
{
    public int StockId { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }

    public int Quantity { get; set; }

    //SQLite specific concurrency token for optimistic concurrency control
    [ConcurrencyCheck]
    public Guid Version { get; set; }

    // SQL Server specific concurrency token for optimistic concurrency control

    //[Timestamp]
    //public byte[] RowVersion { get; set; } 
}
