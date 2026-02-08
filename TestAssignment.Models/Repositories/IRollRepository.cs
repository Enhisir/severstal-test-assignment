using TestAssignment.Models.Models;
using TestAssignment.Models.Utils;

namespace TestAssignment.Models.Repositories;

public interface IRollRepository
{
    public Task<Maybe<Roll>> CreateAsync(Roll roll);
    
    public IQueryable<Roll> BulkGet();
    
    public Task<Maybe<Roll>> DeleteAsync(Guid id);
}