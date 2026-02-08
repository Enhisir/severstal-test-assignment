using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAssignment.Extensions;
using TestAssignment.Models.Database;
using TestAssignment.Models.Models;
using TestAssignment.Services.Abstractions;

namespace TestAssignment.Services;

public class RollStatsService(
    AppDbContext dbContext)
    : IRollStatsService

{
    // для SQL-Based решений
    /*
    public async Task<ActionResult<RollStats>> GetByPeriod(DateTime startDate, DateTime? endDate)
    {
        var query = await dbContext.Rolls
            .Where(r => r.DateAdded >= startDate)
            .WhereIf(endDate is not null, r => r.DateAdded <= endDate)
            .GroupBy(_ => 1)
            .Select(g => new RollStats
            {
                CountAdded = dbContext.Rolls.Count(r =>
                    r.DateAdded >= startDate && r.DateAdded <= endDate),
                CountDeleted = dbContext.Rolls.Count(r =>
                    r.DateDeleted.HasValue &&
                    r.DateDeleted.Value >= startDate &&
                    r.DateDeleted.Value <= endDate),
                AverageLength = g.Average(r => r.Length),
                AverageWeight = g.Average(r => r.Weight),
                MinGap = dbContext.Rolls
                    .Where(r => r.DateDeleted.HasValue &&
                                r.DateAdded <= endDate &&
                                r.DateDeleted.Value >= startDate)
                    .Select(r => (int?)(r.DateDeleted!.Value - r.DateAdded).TotalDays)
                    .DefaultIfEmpty()
                    .Min() ?? 0,
                MaxGap = dbContext.Rolls
                    .Where(r => r.DateDeleted.HasValue &&
                                r.DateAdded <= endDate &&
                                r.DateDeleted >= startDate)
                    .Max(r => (int?)(r.DateDeleted!.Value - r.DateAdded).TotalDays) ?? 0
            })
            .SingleAsync();

        return new OkObjectResult(query);
    }
    */

    // для In-Memory Database
    public async Task<ActionResult<RollStats>> GetByPeriod(DateTime startDate, DateTime? endDate)
    {
        // 1. Основная статистика
        var rollsInPeriod = await dbContext.Rolls
            .Where(r => r.DateAdded <= endDate &&
                        (r.DateDeleted == null || r.DateDeleted >= startDate))
            .ToListAsync(); // Материализуем в память

        // 2. Добавленные в период
        var countAdded = await dbContext.Rolls
            .CountAsync(r => r.DateAdded >= startDate && r.DateAdded <= endDate);

        // 3. Удаленные в период
        var countDeleted = await dbContext.Rolls
            .CountAsync(r => r.DateDeleted.HasValue &&
                             r.DateDeleted.Value >= startDate &&
                             r.DateDeleted.Value <= endDate);

        // 4. Промежутки - вычисляем в памяти
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