namespace Item.Api.Data
{
    public class Category : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<PurchasedProduct> Products { get; } = new List<PurchasedProduct>();

    }
}
