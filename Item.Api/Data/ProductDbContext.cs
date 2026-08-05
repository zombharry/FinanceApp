using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

            //SQL Server specific configuration for RowVersion

            //entity.Property(s => s.RowVersion)
            //.IsRowVersion()
            //.IsConcurrencyToken();
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter("SoftDelete", CreateSoftDeleteFilter(entityType.ClrType));
            }
        }

    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();

        foreach (var item in ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Deleted))
        {
            item.State = EntityState.Modified;
            item.CurrentValues["IsDeleted"] = true;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");

        var isDeleted = Expression.Property(
            parameter,
            nameof(BaseEntity.IsDeleted));

        var condition = Expression.Equal(
            isDeleted,
            Expression.Constant(false));

        return Expression.Lambda(condition, parameter);
    }
}

public static class BaseEntityFilters
{
    public const string SoftDeleteFilter = "SoftDeleteFilter";
}
