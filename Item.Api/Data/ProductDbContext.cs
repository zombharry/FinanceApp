using Microsoft.EntityFrameworkCore;

namespace Item.Api.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();

    public DbSet<PurchasedProduct> PurchasedProducts => Set<PurchasedProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
        });

        modelBuilder.Entity<PurchasedProduct>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(s => s.StockId);
            entity.HasIndex(s => s.ProductId).IsUnique();

            entity.HasOne(s => s.Product)
            .WithOne()
            .HasForeignKey<Stock>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
