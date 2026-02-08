using TestAssignment.Models.Models;

namespace TestAssignment.Services.Abstractions;

public interface IRollStatsService
{
    public Task<RollStats> GetByPeriod(DateTime startDate, DateTime? endDate);
}