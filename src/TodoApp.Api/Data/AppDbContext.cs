using Microsoft.EntityFrameworkCore;

namespace TodoApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>()
            .ToTable("Todos")
            .HasKey(t => t.Id);
            
        modelBuilder.Entity<Todo>()
            .Property(t => t.Id)
            .UseIdentityColumn();
    }
}