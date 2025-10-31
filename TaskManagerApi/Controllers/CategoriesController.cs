using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.DTOs.Categories;
using TaskManagerApi.DTOs.Common;
using TaskManagerApi.Services.Extensions;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableCors("AllowAllHeaders")]
public class CategoriesController(TaskDbContext db) : ControllerBase
{
    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryDto>>> List([FromQuery] CategoryQuery query, CancellationToken ct)
    {
        var uid = User.RequireUserId();

        var q = db.Categories.AsNoTracking().Where(c => c.UserId == uid);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            q = q.Where(c => EF.Functions.ILike(c.Description ?? string.Empty, pattern));
        }

        var sortBy  = (query.SortBy ?? "id").ToLowerInvariant();
        var sortDir = (query.SortDir ?? "asc").ToLowerInvariant();
        q = (sortBy, sortDir) switch
        {
            ("description", "desc") => q.OrderByDescending(c => c.Description),
            ("description", _)      => q.OrderBy(c => c.Description),
            ("id", "desc")          => q.OrderByDescending(c => c.Id),
            _                       => q.OrderBy(c => c.Id),
        };

        var page     = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var total = await q.CountAsync(ct);
        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryDto(c.Id, c.Description))
            .ToListAsync(ct);

        return Ok(new PagedResponse<CategoryDto>(items, page, pageSize, total));
    }

    // GET: api/categories/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
    {
        var uid = User.RequireUserId();
        var c = await db.Categories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == uid && x.Id == id, ct);
        if (c is null) return NotFound();

        return Ok(new CategoryDto(c.Id, c.Description));
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryCreate input, CancellationToken ct)
    {
        var uid = User.RequireUserId();

        var nextId = (await db.Categories
            .Where(c => c.UserId == uid)
            .MaxAsync(c => (int?)c.Id, ct) ?? 0) + 1;

        var entity = new Models.Category
        {
            UserId = uid,
            Id = nextId,
            Description = input.Description
        };

        db.Categories.Add(entity);
        await db.SaveChangesAsync(ct);

        var dto = new CategoryDto(entity.Id, entity.Description);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    // PUT: api/categories/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryUpdate input, CancellationToken ct)
    {
        var uid = User.RequireUserId();
        var entity = await db.Categories.FirstOrDefaultAsync(x => x.UserId == uid && x.Id == id, ct);
        if (entity is null) return NotFound();

        entity.Description = input.Description;
        await db.SaveChangesAsync(ct);

        return Ok(new CategoryDto(entity.Id, entity.Description));
    }

    // DELETE: api/categories/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var uid = User.RequireUserId();
        var entity = await db.Categories.FirstOrDefaultAsync(x => x.UserId == uid && x.Id == id, ct);
        if (entity is null) return NotFound();

        db.Categories.Remove(entity);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return Conflict("Não é possível excluir: existem tarefas vinculadas a esta categoria.");
        }

        return NoContent();
    }
}