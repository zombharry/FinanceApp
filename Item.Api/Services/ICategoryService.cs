using Item.Api.Data;

namespace Item.Api.Services;

public interface ICategoryService
{
    public Task<IEnumerable<Category>> GetAllCategoriesAsync();

    public Task<Category> GetCategoryByIdAsync(string id);

    public Task CreateCategoryAsync(Category category);

    public Task EditCategoryAsync(Category category);

    public Task DeleteCategoryAsync(string id);
}
