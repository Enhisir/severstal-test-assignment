using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TestAssignment.Models.Database;
using TestAssignment.Models.Repositories;
using TestAssignment.Services;
using TestAssignment.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddLogging()
    .AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestAssignment")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging(), 
        ServiceLifetime.Singleton)
    .AddScoped<IRollRepository, RollRepository>()
    .AddScoped<IRollService, RollService>()
    .AddScoped<IRollStatsService, RollStatsService>();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "Severstal Test Assignment";
        opt.Theme = ScalarTheme.Mars;
        opt.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11);
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();