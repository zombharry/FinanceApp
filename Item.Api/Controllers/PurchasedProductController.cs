using Item.Api.Data;
using Item.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Item.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasedProductController : ControllerBase
{
    private readonly ProductDbContext _db;

    public PurchasedProductController(ProductDbContext db)
    {
        _db = db;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.PurchasedProducts.ToListAsync();
        var result = items.Select(p => new PurchasedProductGetDTO
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CategoryId = p.CategoryId,
            PriceAtPurchase = p.PriceAtPurchase,
            Amount = p.Amount,
            OwnerId = p.OwnerId
        });

        return Ok(result);
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var p = await _db.PurchasedProducts.FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();

        var dto = new PurchasedProductGetDTO
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CategoryId = p.CategoryId,
            PriceAtPurchase = p.PriceAtPurchase,
            Amount = p.Amount,
            OwnerId = p.OwnerId
        };

        return Ok(dto);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(PurchasedProductCreateDTO create)
    {
        var entity = new PurchasedProduct
        {
            Id = Guid.NewGuid(),
            ProductId = create.ProductId,
            Name = create.Name,
            Description = create.Description,
            PriceAtPurchase = create.PriceAtPurchase,
            Amount = create.Amount,
            OwnerId = create.OwnerId,
            CategoryId = create.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _db.PurchasedProducts.Add(entity);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("edit")]
    public async Task<IActionResult> Edit(PurchasedProductEditDTO edit)
    {
        if (!Guid.TryParse(edit.Id, out var id))
            return BadRequest();

        var existing = await _db.PurchasedProducts.FirstOrDefaultAsync(x => x.Id == id);
        if (existing is null) return NotFound();

        existing.ProductId = edit.ProductId;
        existing.Name = edit.Name;
        existing.Description = edit.Description;
        existing.PriceAtPurchase = edit.PriceAtPurchase;
        existing.Amount = edit.Amount;
        existing.OwnerId = edit.OwnerId;
        existing.CategoryId = edit.CategoryId;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.PurchasedProducts.FirstOrDefaultAsync(x => x.Id == id);
        if (existing is null) return NotFound();

        _db.PurchasedProducts.Remove(existing);
        await _db.SaveChangesAsync();
        return Ok();
    }
}
