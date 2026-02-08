using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TestAssignment.Models.Models;
using TestAssignment.Services.Abstractions;

namespace TestAssignment.Controllers;

[ApiController]
[Route("rolls/stats")]
public class RollStatsController(IRollStatsService rollStatsService)
{
    [HttpGet]
    public async Task<ActionResult<RollStats>> Get(
        [FromQuery] [Required] DateTime startDate,
        [FromQuery] [Required] DateTime? endDate)
        => await rollStatsService.GetByPeriod(startDate, endDate);
}