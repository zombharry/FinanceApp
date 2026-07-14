using Item.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Item.Api.Services
{
    

    public class ProductService : IProductService
    {
        private readonly ItemDbContext _context;
        public ProductService(ItemDbContext context)
        {
            _context = context;
        }

        public async Task CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(string id)
        {
            await _context.Products.Where(p => p.Id.ToString().Equals(id)).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        public async Task EditProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);

            if (existingProduct is null)
            {
                return;
            }
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<Product>> GetAllProductAsync()
        {
            var products = await _context.Products.ToListAsync();

            return products;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id.ToString().Equals(id));
        }

        public async Task<IEnumerable<Product>> GetUserProductAsync(string userId)
        {
            var products = await _context.Products.Where(p => p.OwnerId == userId).ToListAsync();

            return products;
        }
    }
}
