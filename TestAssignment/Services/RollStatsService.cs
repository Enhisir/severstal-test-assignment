using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAssignment.Common.Models;
using TestAssignment.Data.Database;
using TestAssignment.Services.Abstractions;

namespace TestAssignment.Services;

public class RollStatsService(
    BaseDbContext dbContext)
    : IRollStatsService

{
    public async Task<ActionResult<RollStats>> GetByPeriod(DateTime startDate, DateTime? endDate)
    {
        endDate ??= DateTime.Now;

        var rollsInPeriod = await dbContext.Rolls
            .Where(r => r.DateAdded <= endDate &&
                        (r.DateDeleted == null || r.DateDeleted >= startDate))
            .ToListAsync();

        var countAdded = await dbContext.Rolls
            .CountAsync(r => r.DateAdded >= startDate && r.DateAdded <= endDate);

        var countDeleted = await dbContext.Rolls
            .CountAsync(r => r.DateDeleted.HasValue &&
                             r.DateDeleted.Value >= startDate &&
                             r.DateDeleted.Value <= endDate);

        var deletedRolls = await dbContext.Rolls
            .Where(r => r.DateDeleted.HasValue &&
                        r.DateAdded <= endDate &&
                        r.DateDeleted.Value >= startDate)
            .ToListAsync();

        var gaps = deletedRolls
            .Select(r => (int)(r.DateDeleted!.Value - r.DateAdded).TotalDays)
            .ToList();

        var stats = new RollStats
        {
            CountAdded = countAdded,
            CountDeleted = countDeleted,
            AverageLength = rollsInPeriod.Any() ? rollsInPeriod.Average(r => r.Length) : 0,
            AverageWeight = rollsInPeriod.Any() ? rollsInPeriod.Average(r => r.Weight) : 0,
            MinGap = gaps.Any() ? gaps.Min() : 0,
            MaxGap = gaps.Any() ? gaps.Max() : 0
        };
        return new OkObjectResult(stats);
    }
}