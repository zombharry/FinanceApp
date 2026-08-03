namespace Item.Api.Data;

public class Product : BaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double ? Price { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; }
}
