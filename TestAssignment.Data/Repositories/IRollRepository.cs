using TestAssignment.Common.Models;
using TestAssignment.Common.Utils;

namespace TestAssignment.Data.Repositories;

public interface IRollRepository
{
    public Task<Maybe<Roll>> CreateAsync(Roll roll);
    
    public IQueryable<Roll> BulkGet();
    
    public Task<Maybe<Roll>> DeleteAsync(Guid id);
}