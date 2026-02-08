using TestAssignment.Common.Models;
using TestAssignment.Common.Utils;
using TestAssignment.Data.Database;
using TestAssignment.Data.Repositories;

namespace TestAssignment.Tests.Utils;

public class TestRollRepository : IRollRepository
{
    private readonly BaseDbContext _context;

    public TestRollRepository(BaseDbContext context)
    {
        _context = context;
    }

    public IQueryable<Roll> BulkGet()
        => _context.Rolls.AsQueryable();

    public Task<Maybe<Roll>> CreateAsync(Roll roll)
        => throw new NotImplementedException();

    public Task<Maybe<Roll>> DeleteAsync(Guid id)
        => throw new NotImplementedException();
}