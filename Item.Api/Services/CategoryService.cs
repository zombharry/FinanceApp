using Item.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Item.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ProductDbContext _context;

        public CategoryService(ProductDbContext context)
        {
            _context = context;
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            await _context.Products.Where(p => p.Id.ToString().Equals(id)).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        public async Task EditCategoryAsync(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.Id);

            if (existingCategory is null)
            {
                return;
            }
            existingCategory.Name = category.Name;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();

            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(string id)
        {
            return await _context.Categories.FirstOrDefaultAsync(p => p.Id.ToString().Equals(id));
        }

        public async Task GetAmount()
        {

        }
    }
}
