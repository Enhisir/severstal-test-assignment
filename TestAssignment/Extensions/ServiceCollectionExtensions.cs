using Microsoft.EntityFrameworkCore;
using TestAssignment.Middleware;
using TestAssignment.Data.Database;
using TestAssignment.Data.Repositories;
using TestAssignment.Services;
using TestAssignment.Services.Abstractions;

namespace TestAssignment.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseConfigured(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            // .AddDbContext<AppDbContext>(options =>
            //     options.UseInMemoryDatabase("TestAssignment")
            //         .LogTo(Console.WriteLine, LogLevel.Information)
            //         .EnableSensitiveDataLogging(),
            // ServiceLifetime.Singleton);
            .AddDbContext<BaseDbContext, AppDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection"))
                    .UseSnakeCaseNamingConvention());
    
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        => services
            .AddTransient<ExceptionHandlingMiddleware>();

    public static IServiceCollection AddServices(this IServiceCollection services)
        => services
            .AddTransient<ExceptionHandlingMiddleware>()
            .AddScoped<IRollRepository, RollRepository>()
            .AddScoped<IRollService, RollService>()
            .AddScoped<IRollStatsService, RollStatsService>();
}