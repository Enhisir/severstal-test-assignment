using Microsoft.EntityFrameworkCore;
using TestAssignment.Data.Database;

namespace TestAssignment.Tests.Utils;

public class TestDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options);