using Item.Api.Services;
using Item.Api.DTO;
using Item.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Item.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAllCategory()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        var result = categories.Select(c => new CategoryGetDTO
        {
            CategoryId = c.Id.ToString(),
            CategoryName = c.Name
        });

        return Ok(result);
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetCategoryl(string categoryId)
    {
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);

        if (category is null)
            return NotFound();

        var dto = new CategoryGetDTO
        {
            CategoryId = category.Id.ToString(),
            CategoryName = category.Name
        };

        return Ok(dto);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCategory(CategoryCreateDTO categoryCreateDto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = categoryCreateDto.CategoryName
        };

        await _categoryService.CreateCategoryAsync(category);
        return Ok();
    }

    [HttpPost("edit")]
    public async Task<IActionResult> EditCategory(CategoryEditDTO categoryEditDto)
    {
        var category = new Category
        {
            Id = Guid.Parse(categoryEditDto.CategoryId),
            Name = categoryEditDto.CategoryName
        };

        await _categoryService.EditCategoryAsync(category);
        return Ok();
    }

    [HttpGet("delete")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return Ok();
    }
}
