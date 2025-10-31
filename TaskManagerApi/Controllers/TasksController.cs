using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.DTOs.Common;
using TaskManagerApi.DTOs.Queries;
using TaskManagerApi.DTOs.Tasks;
using TaskManagerApi.Services.Extensions;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController(TaskDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<TaskDto>>> List([FromQuery] TaskQuery query, CancellationToken ct)
    {
        var currentUserId = User.RequireUserId();

        // Sempre começa pelas tarefas do usuário autenticado
        var q = db.Tasks.AsNoTracking().Where(t => t.UserId == currentUserId);

        // Filtros (sem UserId vindo do cliente)
        if (query.CategoryId.HasValue)
            q = q.Where(t => t.CategoryId == query.CategoryId.Value);

        if (query.IsCompleted.HasValue)
            q = q.Where(t => t.IsCompleted == query.IsCompleted.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            q = q.Where(t => EF.Functions.ILike(t.Title, pattern) ||
                             EF.Functions.ILike(t.Description, pattern));
        }

        // Ordenação
        var sortBy  = (query.SortBy ?? "updatedAt").ToLowerInvariant();
        var sortDir = (query.SortDir ?? "desc").ToLowerInvariant();
        q = (sortBy, sortDir) switch
        {
            ("title", "asc")     => q.OrderBy(t => t.Title),
            ("title", _)         => q.OrderByDescending(t => t.Title),
            ("created", "asc")   => q.OrderBy(t => t.Created),
            ("created", _)       => q.OrderByDescending(t => t.Created),
            ("updatedat", "asc") => q.OrderBy(t => t.UpdatedAt),
            ("updatedat", _)     => q.OrderByDescending(t => t.UpdatedAt),
            _                    => q.OrderByDescending(t => t.UpdatedAt)
        };

        // Paginação
        var page     = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var total = await q.CountAsync(ct);
        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskDto(
                t.Id, t.UserId, t.Title, t.Description, t.CategoryId,
                t.IsCompleted, t.Created, t.UpdatedAt))
            .ToListAsync(ct);

        return Ok(new PagedResponse<TaskDto>(items, page, pageSize, total));
    }


    // GET: api/tasks/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id, CancellationToken ct)
    {
        var currentUserId = User.RequireUserId();
        var t = await db.Tasks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserId, ct);
        if (t is null) return NotFound();
        
        var dto = new TaskDto(
            Id: t.Id,
            UserId: t.UserId,
            Title: t.Title.ToString(),
            Description: t.Description,
            CategoryId: t.CategoryId,
            IsCompleted: t.IsCompleted,
            Created: t.Created,
            UpdatedAt: t.UpdatedAt
        );
        return Ok(dto);
    }

    // POST: api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] TaskCreate input, CancellationToken ct)
    {
        var currentUserId = User.RequireUserId();
        
        // Recuse se o corpo vier com outro UserId
        if (input.UserId != currentUserId)
            return Forbid();

        // Validar se a Category pertence ao mesmo User:
        if (input.CategoryId.HasValue)
        {
            var catExists = await db.Categories
                .AnyAsync(c => c.UserId == input.UserId && c.Id == input.CategoryId.Value, ct);
            if (!catExists)
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Categoria inexistente para este usuário.");
                return ValidationProblem(ModelState);
            }
        }

        var now = DateTime.UtcNow;
        var entity = new Models.Task
        {
            Id = Guid.NewGuid(),
            UserId = input.UserId,
            Title = input.Title,               
            Description = input.Description,
            IsCompleted = input.IsCompleted,
            CategoryId = input.CategoryId ?? 0,
            Created = now,
            UpdatedAt = now
        };

        db.Tasks.Add(entity);
        await db.SaveChangesAsync(ct);

        var dto = new TaskDto(
            Id: entity.Id,
            UserId: entity.UserId,
            Title: entity.Title.ToString(),
            Description: entity.Description,
            CategoryId: entity.CategoryId,
            IsCompleted: entity.IsCompleted,
            Created: entity.Created,
            UpdatedAt: entity.UpdatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    // PUT: api/tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid id, [FromBody] TaskUpdate input, CancellationToken ct)
    {
        var currentUserId = User.RequireUserId();
        var entity = await db.Tasks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserId, ct);
        if (entity is null) return NotFound();
        
        if (input.CategoryId.HasValue)
        {
            var catOk = await db.Categories
                .AnyAsync(c => c.UserId == entity.UserId && c.Id == input.CategoryId.Value, ct);
            if (!catOk)
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Categoria inexistente para este usuário.");
                return ValidationProblem(ModelState);
            }
            entity.CategoryId = input.CategoryId.Value;
        }
        
        entity.Title       = input.Title;
        entity.Description = input.Description;
        entity.IsCompleted = input.IsCompleted;
        entity.UpdatedAt   = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        var dto = new TaskDto(
            Id: entity.Id,
            UserId: entity.UserId,
            Title: entity.Title.ToString(),
            Description: entity.Description,
            CategoryId: entity.CategoryId,
            IsCompleted: entity.IsCompleted,
            Created: entity.Created,
            UpdatedAt: entity.UpdatedAt
        );

        return Ok(dto);
    }

    // DELETE: api/tasks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var currentUserId = User.RequireUserId();
        var entity = await db.Tasks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserId, ct);
        if (entity is null) return NotFound();


        db.Tasks.Remove(entity);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}