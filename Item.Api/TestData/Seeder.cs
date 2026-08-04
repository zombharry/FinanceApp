using Item.Api.Data;
using Bogus;

namespace Item.Api.TestData;

public class Seeder
{
    public void Seed()
    {
        var categoryFaker = new Faker<Category>()
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1));

        var categories = categoryFaker.Generate(10);

        var productFaker = new Faker<Product>()
        .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => double.Parse(f.Commerce.Price()))
            .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id)
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(1));

        var products = productFaker.Generate(100);


        var purchasedProductFaker = new Faker<PurchasedProduct>()
            .RuleFor(pp => pp.Id, f => Guid.NewGuid())
            .RuleFor(pp => pp.OwnerId, f => f.PickRandom(SeedUserId.All))
            .RuleFor(pp => pp.CreatedAt, f => f.Date.Recent(60));


        var faker = new Faker();
        var purchasedProducts = new List<PurchasedProduct>();

        foreach (var product in faker.PickRandom(products, 50))
        {
            var purchase = purchasedProductFaker.Generate();
            purchase.ProductId = product.Id;
            purchase.PriceAtPurchase = product.Price ?? 0;
            purchasedProducts.Add(purchase);
        }
    }
}
