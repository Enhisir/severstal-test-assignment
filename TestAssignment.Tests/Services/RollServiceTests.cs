using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TestAssignment.Common.Models;
using TestAssignment.Common.Utils;
using TestAssignment.Data.Database;
using TestAssignment.Data.Repositories;
using TestAssignment.Dtos;
using TestAssignment.Services;
using TestAssignment.Tests.Utils;

namespace TestAssignment.Tests.Services;

public class RollServiceTests
{
    private readonly Mock<IRollRepository> _repositoryMock;
    private readonly RollService _service;

    public RollServiceTests()
    {
        _repositoryMock = new Mock<IRollRepository>();
        var loggerMock = new Mock<ILogger<RollService>>();
        _service = new RollService(_repositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_InvalidDto_ReturnsBadRequest()
    {
        var dto = new CreateRollDto();

        var result = await _service.CreateAsync(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<object>>(badRequest.Value);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Roll>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_RepositorySuccess_ReturnsOkWithRoll()
    {
        var dto = new CreateRollDto
        {
            Length = 10,
            Weight = 5
        };

        var repositoryResult = Maybe<Roll>.Success(new Roll
        {
            Id = Guid.NewGuid(),
            Length = dto.Length,
            Weight = dto.Weight,
            DateAdded = DateTime.UtcNow
        });

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Roll>()))
            .ReturnsAsync(repositoryResult);

        var result = await _service.CreateAsync(dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var roll = Assert.IsType<Roll>(ok.Value);
        Assert.Equal(dto.Length, roll.Length);
        Assert.Equal(dto.Weight, roll.Weight);
    }

    [Fact]
    public async Task CreateAsync_RepositoryError_ReturnsBadRequest()
    {
        var dto = new CreateRollDto
        {
            Length = 10,
            Weight = 5
        };

        var repositoryResult = Maybe<Roll>.Failure("create_error");

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Roll>()))
            .ReturnsAsync(repositoryResult);

        var result = await _service.CreateAsync(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var details = Assert.IsType<ProblemDetails>(badRequest.Value);
        Assert.Equal(repositoryResult.Error, details.Instance);
    }
    
    [Fact]
    public async Task BulkGetAsync_ReturnsFilteredRolls()
    {
        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TestDbContext(options);

        context.Rolls.AddRange(
            new Roll { Id = Guid.NewGuid(), Length = 10, Weight = 5 },
            new Roll { Id = Guid.NewGuid(), Length = 20, Weight = 8 }
        );

        await context.SaveChangesAsync();

        var repository = new TestRollRepository(context);
        var service = new RollService(repository, Mock.Of<ILogger<RollService>>());

        var result = await service.BulkGetAsync(
            id: null,
            minLength: 15,
            maxLength: null,
            minWidth: null,
            maxWidth: null,
            dateAdded: null,
            dateRemoved: null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var rolls = Assert.IsAssignableFrom<IEnumerable<Roll>>(ok.Value);
        var collection = rolls.ToList();
        
        Assert.Single(collection);
        Assert.Equal(20, collection.First().Length);
    }


    [Fact]
    public async Task DeleteAsync_Success_ReturnsOk()
    {
        var roll = new Roll
        {
            Id = Guid.NewGuid(),
            DateAdded = DateTime.UtcNow,
            DateDeleted = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.DeleteAsync(roll.Id))
            .ReturnsAsync(Maybe<Roll>.Success(roll));

        var result = await _service.DeleteAsync(roll.Id);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRoll = Assert.IsType<Roll>(ok.Value);
        Assert.Equal(roll.Id, returnedRoll.Id);
        Assert.Equal(roll.DateDeleted, returnedRoll.DateDeleted);
    }

    [Fact]
    public async Task DeleteAsync_Error_ReturnsBadRequest()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.DeleteAsync(id))
            .ReturnsAsync(Maybe<Roll>.Failure("delete_error"));

        var result = await _service.DeleteAsync(id);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var details = Assert.IsType<ProblemDetails>(badRequest.Value);
        Assert.Equal("delete_error", details.Instance);
    }
}