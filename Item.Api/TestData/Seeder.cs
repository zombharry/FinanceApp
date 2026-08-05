using Item.Api.Data;
using Bogus;

namespace Item.Api.TestData;

public class Seeder
{
    private readonly ProductDbContext _db;

    public Seeder(ProductDbContext db)
    {
        _db = db;
    }

    public void Seed()
    {
        if (_db.Categories.Any()) 
        {
            return;
        }

        var faker = new Faker();

        var categoryFaker = new Faker<Category>()
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1));

        var categories = categoryFaker.Generate(10);
        _db.Categories.AddRange(categories);

        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => double.Parse(f.Commerce.Price()))
            .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id)
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(1));

        var products = productFaker.Generate(100);
        _db.Products.AddRange(products);

        var initialStocks = products.Select(p => new Stock
        {
            ProductId = p.Id,
            Quantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = Guid.NewGuid()
            //RowVersion = BitConverter.GetBytes(DateTime.UtcNow.Ticks)
        }).ToList();
        _db.Stocks.AddRange(initialStocks);

        var deliveries = new List<Delivery>();
        foreach (var product in faker.PickRandom(products, 40))
        {
            var amount = faker.Random.Int(1, 20);
            var delivery = new Delivery
            {
                DeliveryId = Guid.NewGuid(),
                DeliveredItemId = product.Id,
                Amount = amount,
                Cost = Math.Round(faker.Random.Double(1, 200), 2),
                CreatedAt = DateTime.UtcNow
            };
            deliveries.Add(delivery);

            var stock = initialStocks.FirstOrDefault(s => s.ProductId == product.Id)
                        ?? _db.Stocks.FirstOrDefault(s => s.ProductId == product.Id);
            if (stock != null)
            {
                stock.Quantity += amount;
                stock.UpdatedAt = DateTime.UtcNow;
            }
        }
        _db.Deliveries.AddRange(deliveries);

        var purchasedProductFaker = new Faker<PurchasedProduct>()
            .RuleFor(pp => pp.Id, f => Guid.NewGuid())
            .RuleFor(pp => pp.OwnerId, f => f.PickRandom(SeedUserId.All))
            .RuleFor(pp => pp.CreatedAt, f => f.Date.Recent(60));

        var purchasedProducts = new List<PurchasedProduct>();
        foreach (var product in faker.PickRandom(products, 50))
        {
            var purchase = purchasedProductFaker.Generate();
            purchase.ProductId = product.Id;
            purchase.PriceAtPurchase = product.Price ?? 0;
            purchase.Name = product.Name;
            purchase.Description = product.Description;
            purchase.CategoryId = product.CategoryId;
            purchase.Amount = faker.Random.Int(1, 5);

            // decrement stock
            var stock = initialStocks.FirstOrDefault(s => s.ProductId == product.Id)
                        ?? _db.Stocks.FirstOrDefault(s => s.ProductId == product.Id);
            if (stock != null)
            {
                stock.Quantity = Math.Max(0, stock.Quantity - purchase.Amount);
                stock.UpdatedAt = DateTime.UtcNow;
            }

            purchasedProducts.Add(purchase);
        }
        _db.PurchasedProducts.AddRange(purchasedProducts);

        _db.SaveChanges();
    }
}
