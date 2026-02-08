using Microsoft.EntityFrameworkCore;
using TestAssignment.Common.Models;

namespace TestAssignment.Data.Database;

public abstract class BaseDbContext(DbContextOptions options)
    : DbContext(options)
{
    public DbSet<Roll> Rolls { get; set; } = null!;
}