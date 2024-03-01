using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TodoDatabaseDbContext(DbContextOptions<TodoDatabaseDbContext> options) : DbContext(options)
{
    public DbSet<Todo> TodoItems => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        DefineTodoType(builder.Entity<Todo>());
    }

    private static void DefineTodoType(EntityTypeBuilder<Todo> builder)
    {
        builder.ToTable("todo");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .UseHiLo("todo_type_hilo")
            .IsRequired();

        builder.Property(cb => cb.Description)
            .IsRequired()
            .HasMaxLength(128);
    }
}