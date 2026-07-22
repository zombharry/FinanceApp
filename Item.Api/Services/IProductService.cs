using Item.Api.Data;

namespace Item.Api.Services
{
    public interface IProductService
    {
        public Task<IEnumerable<Product>> GetAllProductAsync();

        public Task<IEnumerable<Product>> GetUserProductAsync(string userId);

        public Task<Product> GetProductByIdAsync(Guid id);

        public Task CreateProductAsync(Product product);

        public Task EditProductAsync(Product product);

        public Task DeleteProductAsync(Guid id);
    }
}
