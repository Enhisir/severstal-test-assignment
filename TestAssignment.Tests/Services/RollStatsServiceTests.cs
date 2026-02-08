using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAssignment.Common.Models;
using TestAssignment.Data.Database;
using TestAssignment.Services;
using TestAssignment.Tests.Utils;

namespace TestAssignment.Tests.Services;

public class RollStatsServiceTests
{
    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

    [Fact]
    public async Task GetByPeriod_ReturnsCorrectStats()
    {
        await using var context = CreateContext();
        
        var start = new DateTime(2024, 01, 01);
        var end = new DateTime(2024, 01, 31);

        context.Rolls.AddRange(
            new Roll
            {
                Length = 10,
                Weight = 5,
                DateAdded = start.AddDays(4),
                DateDeleted = start.AddDays(9)
            },
            new Roll
            {
                Length = 20,
                Weight = 7,
                DateAdded = start.AddDays(10)
            },
            new Roll
            {
                Length = 30,
                Weight = 9,
                DateAdded = start.AddMonths(1) // вне периода
            }
        );

        await context.SaveChangesAsync();

        var service = new RollStatsService(context);

        var result = await service.GetByPeriod(start, end);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var stats = Assert.IsType<RollStats>(ok.Value);

        Assert.Equal(2, stats.CountAdded);
        Assert.Equal(1, stats.CountDeleted);
        Assert.Equal(15, stats.AverageLength);
        Assert.Equal(6, stats.AverageWeight);
        Assert.Equal(5, stats.MinGap);
        Assert.Equal(5, stats.MaxGap);
    }

    [Fact]
    public async Task GetByPeriod_EndDateNull_WorksCorrectly()
    {
        await using var context = CreateContext();

        var start = new DateTime(2024, 01, 01);

        context.Rolls.AddRange(
            new Roll
            {
                Length = 10,
                Weight = 5,
                DateAdded = start.AddDays(1),
                DateDeleted = start.AddDays(3)
            },
            new Roll
            {
                Length = 20,
                Weight = 7,
                DateAdded = start.AddDays(5)
            }
        );

        await context.SaveChangesAsync();

        var service = new RollStatsService(context);

        var result = await service.GetByPeriod(start, null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var stats = Assert.IsType<RollStats>(ok.Value);

        Assert.Equal(2, stats.CountAdded);
        Assert.Equal(1, stats.CountDeleted);
        Assert.Equal(15, stats.AverageLength);
        Assert.Equal(6, stats.AverageWeight);
    }

    [Fact]
    public async Task GetByPeriod_NoDeletedRolls_GapsAreZero()
    {
        await using var context = CreateContext();

        var start = new DateTime(2024, 01, 01);
        var end = new DateTime(2024, 01, 31);

        context.Rolls.Add(
            new Roll
            {
                Length = 10,
                Weight = 5,
                DateAdded = start.AddDays(1)
            }
        );

        await context.SaveChangesAsync();

        var service = new RollStatsService(context);

        var result = await service.GetByPeriod(start, end);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var stats = Assert.IsType<RollStats>(ok.Value);

        Assert.Equal(1, stats.CountAdded);
        Assert.Equal(0, stats.CountDeleted);
        Assert.Equal(0, stats.MinGap);
        Assert.Equal(0, stats.MaxGap);
    }

    [Fact]
    public async Task GetByPeriod_DateBoundaries_AreInclusive()
    {
        await using var context = CreateContext();

        var start = new DateTime(2024, 01, 01);
        var end = new DateTime(2024, 01, 10);

        context.Rolls.AddRange(
            new Roll
            {
                Length = 10,
                Weight = 5,
                DateAdded = start,
                DateDeleted = end
            },
            new Roll
            {
                Length = 20,
                Weight = 7,
                DateAdded = end.AddDays(1)
            }
        );

        await context.SaveChangesAsync();

        var service = new RollStatsService(context);

        var result = await service.GetByPeriod(start, end);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var stats = Assert.IsType<RollStats>(ok.Value);

        Assert.Equal(1, stats.CountAdded);
        Assert.Equal(1, stats.CountDeleted);
    }
}