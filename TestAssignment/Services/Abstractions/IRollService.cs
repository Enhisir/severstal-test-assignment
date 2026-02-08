using Microsoft.AspNetCore.Mvc;
using TestAssignment.Common.Models;
using TestAssignment.Dtos;

namespace TestAssignment.Services.Abstractions;

public interface IRollService
{
    public Task<ActionResult<Roll>> CreateAsync(CreateRollDto dto);

    public Task<ActionResult<IEnumerable<Roll>>> BulkGetAsync(
        Guid? id,
        double? minLength,
        double? maxLength,
        double? minWidth,
        double? maxWidth,
        DateTime? dateAdded,
        DateTime? dateRemoved);

    public Task<ActionResult<Roll>> DeleteAsync(Guid rollId);
}