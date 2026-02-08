using Scalar.AspNetCore;
using TestAssignment.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddLogging()
    .AddOpenApi()
    .AddDatabaseConfigured(builder.Configuration)
    .AddExceptionHandling()
    .AddServices()
    .AddControllers();

var app = builder.Build();

app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "Severstal Test Assignment";
        opt.Theme = ScalarTheme.Mars;
        opt.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.Http, ScalarClient.Http11);
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();