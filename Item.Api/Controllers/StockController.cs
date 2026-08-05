using Item.Api.Data;
using Item.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Item.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly ProductDbContext _db;

    public StockController(ProductDbContext db)
    {
        _db = db;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Stocks.ToListAsync();
        var result = items.Select(s => new StockGetDTO
        {
            StockId = s.StockId,
            ProductId = s.ProductId,
            Quantity = s.Quantity
        });
        return Ok(result);
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById(int stockId)
    {
        var s = await _db.Stocks.FirstOrDefaultAsync(x => x.StockId == stockId);
        if (s is null) return NotFound();

        var dto = new StockGetDTO
        {
            StockId = s.StockId,
            ProductId = s.ProductId,
            Quantity = s.Quantity
        };

        return Ok(dto);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(StockCreateDTO create)
    {
        var entity = new Stock
        {
            ProductId = create.ProductId,
            Quantity = create.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        _db.Stocks.Add(entity);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("edit")]
    public async Task<IActionResult> Edit(StockEditDTO edit)
    {
        var existing = await _db.Stocks.FirstOrDefaultAsync(x => x.StockId == edit.StockId);
        if (existing is null) return NotFound();

        existing.ProductId = edit.ProductId;
        existing.Quantity = edit.Quantity;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _db.Stocks.FirstOrDefaultAsync(x => x.StockId == id);
        if (existing is null) return NotFound();

        _db.Stocks.Remove(existing);
        await _db.SaveChangesAsync();
        return Ok();
    }
}
