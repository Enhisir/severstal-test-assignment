using Microsoft.AspNetCore.Mvc;
using TestAssignment.Dtos;
using TestAssignment.Models.Models;
using TestAssignment.Services.Abstractions;

namespace TestAssignment.Controllers;

[ApiController]
[Route("[controller]")]
public class RollsController(IRollService service)
{
    // добавить список ошибок
    // добавить документацию для методов
    
    [HttpPost]
    public async Task<ActionResult<Roll>> CreateAsync([FromBody] CreateRollDto dto)
        => await service.CreateAsync(dto);
    
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

    [HttpDelete]
    public async Task<ActionResult<Roll>> DeleteAsync([FromBody] RemoveRollDto dto)
        => await service.DeleteAsync(dto);
}