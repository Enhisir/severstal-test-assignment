using Microsoft.EntityFrameworkCore;
using TestAssignment.Models.Models;

namespace TestAssignment.Models.Database;

public sealed class AppDbContext : DbContext
{
    public DbSet<Roll> Rolls { get; set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Roll>().HasData(
                new Roll
                {
                    Id = new Guid("689f7f26-8cfc-4d58-81ed-6200a43b0813"),
                    Length = 1,
                    Weight = 1,
                    DateAdded = new DateTime(2026, 1, 1)
                },
                new Roll
                {
                    Id = new Guid("b248061c-1001-482d-8270-af253ee3354d"),
                    Length = 100,
                    Weight = 50,
                    DateAdded = new DateTime(2026, 1, 2)
                },
                new Roll
                {
                    Id = new Guid("40bed416-0384-403c-b16d-c5c77e3aa12c"),
                    Length = 200,
                    Weight = 200,
                    DateAdded = new DateTime(2026, 1, 3)
                });
    }
}