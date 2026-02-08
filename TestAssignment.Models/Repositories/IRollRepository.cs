using TestAssignment.Models.Models;

namespace TestAssignment.Models.Repositories;

public interface IRollRepository
{
    public Task<bool> CreateAsync(Roll roll);
    
    public IQueryable<Roll> BulkGet();
    
    public Task<Roll?> DeleteAsync(Guid id);
}