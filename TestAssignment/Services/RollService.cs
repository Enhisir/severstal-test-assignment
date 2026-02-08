using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAssignment.Common.Models;
using TestAssignment.Dtos;
using TestAssignment.Extensions;
using TestAssignment.Data.Repositories;
using TestAssignment.Services.Abstractions;

namespace TestAssignment.Services;

public class RollService(
    IRollRepository repository,
    ILogger<RollService> logger)
    : IRollService
{
    public async Task<ActionResult<Roll>> CreateAsync(CreateRollDto dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(dto, context, results, true))
        {
            logger.LogInformation("Validation error when trying to create new roll.");
            return new BadRequestObjectResult(results);
        }

        var roll = new Roll
        {
            Id = Guid.NewGuid(),
            Length = dto.Length,
            Weight = dto.Weight,
            DateAdded = DateTime.UtcNow
        };

        var result = await repository.CreateAsync(roll);
        if (result)
        {
            logger.LogInformation("Created new Roll(id={rollId}, dateAdded={dateAdded}).", roll.Id, roll.DateAdded);
            return new OkObjectResult(roll);
        }

        logger.LogInformation("Error when trying to create new roll: {problemDetails}.", result.Error);
        return new BadRequestObjectResult(new ProblemDetails()
        {
            Instance = result.Error
        });
    }

    // Из ТЗ не очень понятно, надо ли показывать удаленные рулоны.
    // По ним почему-то должен быть фильтр
    // Так получилось, что у меня не было времени уточнить это у авторов задания
    // Но при получении такой таски этот момент надо было бы все-таки уточнить заранее
    // Рассматривается вариант решения, где удаленные рулоны показывать все же надо
    /*
    if (maybeEntity?.DateDeleted != null)
    {
        // log tried to get deleted
        return null;
    }
    */
    public async Task<ActionResult<IEnumerable<Roll>>> BulkGetAsync(
        Guid? id,
        double? minLength,
        double? maxLength,
        double? minWidth,
        double? maxWidth,
        DateTime? dateAdded,
        DateTime? dateRemoved)
        => new OkObjectResult(
            await repository
                .BulkGet()
                .WhereIf(id is not null, r => r.Id == id)
                .WhereIf(minLength is not null, r => r.Length >= minLength)
                .WhereIf(maxLength is not null, r => r.Length <= maxLength)
                .WhereIf(minWidth is not null, r => r.Weight >= minWidth)
                .WhereIf(maxWidth is not null, r => r.Weight <= maxWidth)
                .WhereIf(dateAdded is not null, r => r.DateAdded == dateAdded)
                .WhereIf(dateRemoved is not null, r => r.DateAdded == dateRemoved)
                .ToListAsync());

    public async Task<ActionResult<Roll>> DeleteAsync(Guid rollId)
    {
        var result = await repository.DeleteAsync(rollId);

        if (result)
        {
            var roll = result.Value;
            logger.LogInformation(
                "Deleted Roll(id={rollId}, dateAdded={dateAdded}, dateAdded={dateRemoved}).",
                roll.Id,
                roll.DateAdded,
                roll.DateDeleted);
            return new OkObjectResult(roll);
        }

        return new BadRequestObjectResult(new ProblemDetails()
        {
            Instance = result.Error
        });
    }
}