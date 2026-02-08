using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestAssignment.Models.Database;
using TestAssignment.Models.Models;

namespace TestAssignment.Models.Repositories;

public class RollRepository(
    AppDbContext dbContext,
    ILogger<RollRepository> logger)
    : IRollRepository
{
    public async Task<bool> CreateAsync(Roll roll)
    {
        ArgumentNullException.ThrowIfNull(roll);

        if (roll.DateDeleted is not null)
        {
            logger.LogInformation(
                "Tried to create an invalid roll entity, roll cannot have deletion date when being created");
            return false;
        }

        await dbContext.Rolls.AddAsync(roll);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public IQueryable<Roll> BulkGet()
        => dbContext.Rolls.AsQueryable().AsNoTracking();

    public async Task<Roll?> DeleteAsync(Guid id)
    {
        var entity = await dbContext.Rolls
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity is not null)
        {
            entity.DateDeleted = DateTime.UtcNow;
            dbContext.Rolls.Update(entity);
            await dbContext.SaveChangesAsync();
            // log deleted entity
            return entity;
        }

        // log
        return null;
    }
}