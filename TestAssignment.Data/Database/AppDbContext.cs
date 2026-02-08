using Microsoft.EntityFrameworkCore;
using TestAssignment.Common.Models;

namespace TestAssignment.Data.Database;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Roll>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DateAdded)
                .IsRequired()
                .HasDefaultValueSql("TIMEZONE('utc', now())")
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            entity.Property(e => e.DateDeleted)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToUniversalTime() : v,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            entity.HasData(
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
        });
    }
}