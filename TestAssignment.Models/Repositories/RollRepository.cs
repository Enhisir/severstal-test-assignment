using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestAssignment.Models.Database;
using TestAssignment.Models.Models;
using TestAssignment.Models.Utils;

namespace TestAssignment.Models.Repositories;

public class RollRepository(
    AppDbContext dbContext,
    ILogger<RollRepository> logger)
    : IRollRepository
{
    public async Task<Maybe<Roll>> CreateAsync(Roll roll)
    {
        ArgumentNullException.ThrowIfNull(roll);

        if (roll.DateDeleted is not null)
        {
            logger.LogInformation(
                "Tried to create an invalid roll entity, roll cannot have deletion date when being created");
            return Maybe<Roll>.Failure("roll cannot have deletion date when being created");
        }

        await dbContext.Rolls.AddAsync(roll);
        await dbContext.SaveChangesAsync();
        return Maybe<Roll>.Success(roll);
    }

    public IQueryable<Roll> BulkGet()
        => dbContext.Rolls.AsQueryable().AsNoTracking();

    public async Task<Maybe<Roll>> DeleteAsync(Guid id)
    {
        var entity = await dbContext.Rolls
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            return Maybe<Roll>.Failure("Roll not found");
        }

        if (entity.DateDeleted is not null)
        {
            return Maybe<Roll>.Failure("Cannot delete already deleted roll");
        }

        entity.DateDeleted = DateTime.UtcNow;
        dbContext.Rolls.Update(entity);
        await dbContext.SaveChangesAsync();
        logger.LogInformation(
            "Deleted roll(id={rollId}, dateAdded={dateAdded}, dateAdded={dateRemoved}).",
            entity.Id,
            entity.DateAdded,
            entity.DateAdded);
        return Maybe<Roll>.Success(entity);
    }
}