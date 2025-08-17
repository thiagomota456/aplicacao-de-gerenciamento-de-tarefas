using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Models;

namespace TaskManagerApi.Data;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Models.Task> Tasks => Set<Models.Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // USERS
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).IsRequired();
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
            e.ToTable("users");
        });

        // CATEGORIES (PK composta: user_id + id)
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => new { x.UserId, x.Id });
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Id).IsRequired();
            e.Property(x => x.Description);

            e.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.ToTable("categories");
            e.HasIndex(x => new { x.UserId, x.Description });
        });

        // TASKS
        modelBuilder.Entity<Models.Task>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Title).IsRequired();       // Guid (no seu model)
            e.Property(x => x.Description).IsRequired();
            e.Property(x => x.IsCompleted).IsRequired();
            e.Property(x => x.CategoryId);               // int (no seu model, não-nullable)
            e.Property(x => x.Created).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();

            e.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK composta para garantir consistência de categoria do MESMO usuário
            e.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => new { x.UserId, x.CategoryId })
                .OnDelete(DeleteBehavior.Restrict);

            e.ToTable("tasks");
            e.HasIndex(x => new { x.UserId, x.IsCompleted });
            e.HasIndex(x => new { x.UserId, x.CategoryId });
        });
    }
}