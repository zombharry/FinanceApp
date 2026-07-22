using Item.Api.Data;
using Item.Api.DTO;
using Item.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Item.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("getuserproduct")]
    public async Task<IActionResult> GetUserProduct(string userId)
    {
        var products = await _productService.GetUserProductAsync(userId);
        return Ok(products);
    }

    [HttpGet("getallproduct")]
    public async Task<IActionResult> GetAllProduct()
    {
        var products = await _productService.GetAllProductAsync();
        return Ok(products);
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetProduct(string productId)
    {
        var productGet = await _productService.GetProductByIdAsync(productId);
        var productGetDto = new ProductGetDTO
        {
            Id = productGet.Id,
            Name = productGet.Name,
            Description = productGet.Description,
            Price = productGet.Price,
            OwnerId = productGet.OwnerId,
            CategoryId = productGet.CategoryId
        };
        return Ok(productGetDto);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct(ProductCreateDTO productCreateDto)
    {
        var product = new Product
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price,
            OwnerId = productCreateDto.OwnerId,
            CategoryId = productCreateDto.CategoryId
        };

        await _productService.CreateProductAsync(product);
        return Ok();
    }

    [HttpPost("edit")]
    public async Task<IActionResult> EditProduct(ProductEditDTO productEditDto)
    {
        var product = new Product
        {
            Id = Guid.Parse(productEditDto.Id),
            Name = productEditDto.Name,
            Description = productEditDto.Description,
            Price = productEditDto.Price,
            OwnerId = productEditDto.OwnerId,
            CategoryId = productEditDto.CategoryId
        };

        await _productService.EditProductAsync(product);
        return Ok();
    }
    [HttpGet("delete")]
    public async Task<ActionResult> DeleteProduct(string id)
    {
        await _productService.DeleteProductAsync(id);
        return Ok();
    }
}
