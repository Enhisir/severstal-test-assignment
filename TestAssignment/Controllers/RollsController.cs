using Microsoft.AspNetCore.Mvc;
using TestAssignment.Common.Models;
using TestAssignment.Dtos;
using TestAssignment.Services.Abstractions;

namespace TestAssignment.Controllers;

/// <summary>
/// Контроллер для управления рулонами на складе
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class RollsController(IRollService service)
{
    
    /// <summary>
    /// Создание нового рулона
    /// </summary>
    /// <param name="dto">Данные для создания рулона</param>
    /// <returns>Созданный рулон</returns>
    /// <response code="200">Рулон успешно создан</response>
    /// <response code="400">
    /// Неверные данные рулона:
    /// - Некорректная длина или вес
    /// - Указана дата удаления при создании
    /// </response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpPost]
    public async Task<ActionResult<Roll>> CreateAsync([FromBody] CreateRollDto dto)
        => await service.CreateAsync(dto);
    
    /// <summary>
    /// Получение списка рулонов с фильтрацией
    /// </summary>
    /// <param name="id">Фильтр по идентификатору</param>
    /// <param name="minLength">Минимальная длина</param>
    /// <param name="maxLength">Максимальная длина</param>
    /// <param name="minWeight">Минимальный вес</param>
    /// <param name="maxWeight">Максимальный вес</param>
    /// <param name="dateAdded">Дата добавления (точное совпадение)</param>
    /// <param name="dateRemoved">Дата удаления (точное совпадение)</param>
    /// <returns>Список рулонов, соответствующих фильтрам</returns>
    /// <response code="200">Успешный запрос</response>
    /// <remarks>
    /// Примеры запросов:
    /// - GET /api/rolls?minLength=10&maxLength=20 - рулоны длиной от 10 до 20
    /// - GET /api/rolls?dateAdded=2026-01-15 - рулоны, добавленные 15 января 2026
    /// - GET /api/rolls?id=3fa85f64-5717-4562-b3fc-2c963f66afa6 - конкретный рулон
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Roll>>> ListAsync(
        [FromQuery] Guid? id,
        [FromQuery] double? minLength,
        [FromQuery] double? maxLength,
        [FromQuery] double? minWidth,
        [FromQuery] double? maxWidth,
        [FromQuery] DateTime? dateAdded,
        [FromQuery] DateTime? dateRemoved)
        => await service.BulkGetAsync(id, minLength, maxLength, minWidth, maxWidth, dateAdded, dateRemoved);

    /// <summary>
    /// Удаление рулона (мягкое удаление)
    /// </summary>
    /// <param name="rollId">Идентификатор рулона</param>
    /// <returns>Удаленный рулон</returns>
    /// <response code="200">Рулон успешно помечен как удаленный</response>
    /// <response code="404">Рулон не найден</response>
    /// <response code="400">Рулон уже удален</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    /// <remarks>
    /// Внимание: используется мягкое удаление (soft delete). 
    /// Рулон помечается датой удаления, но остается в базе данных.
    /// </remarks>
    [HttpDelete("{rollId}")]
    public async Task<ActionResult<Roll>> DeleteAsync([FromRoute] Guid rollId)
        => await service.DeleteAsync(rollId);
}