using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TestAssignment.Common.Models;
using TestAssignment.Services.Abstractions;

namespace TestAssignment.Controllers;

/// <summary>
/// Контроллер для получения статистики по рулонам
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class RollStatsController(IRollStatsService rollStatsService)
{
    /// <summary>
    /// Получение статистики по рулонам за указанный период
    /// </summary>
    /// <param name="startDate">Начальная дата периода (обязательный параметр)</param>
    /// <param name="endDate">Конечная дата периода (если не указана - используется текущее время)</param>
    /// <returns>Статистика по рулонам за период</returns>
    /// <response code="200">Статистика успешно рассчитана</response>
    /// <response code="400">
    /// Неверные параметры запроса:
    /// - Дата начала периода не указана
    /// - Дата начала позже даты окончания
    /// - Неверный формат даты
    /// </response>
    /// <response code="500">Внутренняя ошибка сервера при расчете статистики</response>
    [HttpGet]
    public async Task<ActionResult<RollStats>> Get(
        [FromQuery] [Required] DateTime startDate,
        [FromQuery] DateTime? endDate = null)
        => await rollStatsService.GetByPeriod(startDate, endDate);
}