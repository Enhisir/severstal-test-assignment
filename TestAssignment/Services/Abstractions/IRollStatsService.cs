using Microsoft.AspNetCore.Mvc;
using TestAssignment.Models.Models;

namespace TestAssignment.Services.Abstractions;

public interface IRollStatsService
{
    public Task<ActionResult<RollStats>> GetByPeriod(DateTime startDate, DateTime? endDate);
}