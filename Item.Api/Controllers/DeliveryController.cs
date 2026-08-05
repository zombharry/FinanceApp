using Item.Api.Data;
using Item.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Item.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryController : ControllerBase
{
    private readonly ProductDbContext _db;

    public DeliveryController(ProductDbContext db)
    {
        _db = db;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Deliveries.ToListAsync();
        var result = items.Select(d => new DeliveryGetDTO
        {
            DeliveryId = d.DeliveryId,
            DeliveredItemId = d.DeliveredItemId,
            Amount = d.Amount,
            Cost = d.Cost
        });
        return Ok(result);
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById(Guid deliveryId)
    {
        var d = await _db.Deliveries.FirstOrDefaultAsync(x => x.DeliveryId == deliveryId);
        if (d is null) return NotFound();

        var dto = new DeliveryGetDTO
        {
            DeliveryId = d.DeliveryId,
            DeliveredItemId = d.DeliveredItemId,
            Amount = d.Amount,
            Cost = d.Cost
        };

        return Ok(dto);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(DeliveryCreateDTO create)
    {
        var entity = new Delivery
        {
            DeliveryId = Guid.NewGuid(),
            DeliveredItemId = create.DeliveredItemId,
            Amount = create.Amount,
            Cost = create.Cost,
            CreatedAt = DateTime.UtcNow
        };

        _db.Deliveries.Add(entity);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("edit")]
    public async Task<IActionResult> Edit(DeliveryEditDTO edit)
    {
        if (!Guid.TryParse(edit.DeliveryId, out var id))
            return BadRequest();

        var existing = await _db.Deliveries.FirstOrDefaultAsync(x => x.DeliveryId == id);
        if (existing is null) return NotFound();

        existing.DeliveredItemId = edit.DeliveredItemId;
        existing.Amount = edit.Amount;
        existing.Cost = edit.Cost;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("delete")]
    public async Task<IActionResult> Delete(Guid deliveryId)
    {
        var existing = await _db.Deliveries.FirstOrDefaultAsync(x => x.DeliveryId == deliveryId);
        if (existing is null) return NotFound();

        _db.Deliveries.Remove(existing);
        await _db.SaveChangesAsync();
        return Ok();
    }
}
