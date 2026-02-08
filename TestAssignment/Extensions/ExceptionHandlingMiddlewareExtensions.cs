using TestAssignment.Middleware;

namespace TestAssignment.Extensions;

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var middleware = scope.ServiceProvider.GetRequiredService<ExceptionHandlingMiddleware>();
        return app.Use(next => context => middleware.InvokeAsync(context, next));
    }
}