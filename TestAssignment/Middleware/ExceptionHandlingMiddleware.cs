using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace TestAssignment.Middleware;

public class ExceptionHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorId = Guid.NewGuid();
        var problemDetails = new ProblemDetails();
        
        switch (exception)
        {
            case DbException dbEx:
                problemDetails.Title = "Database Error";
                problemDetails.Detail = "A database error occurred while processing your request.";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                logger.LogError(
                    dbEx, 
                    "Database update failed - ErrorId: {ErrorId}", 
                    errorId);
                break;
                
            default:
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred.";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                logger.LogError(
                    exception, 
                    "Unhandled exception - ErrorId: {ErrorId}", 
                    errorId);
                break;
        }
        
        problemDetails.Extensions["errorId"] = errorId;
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        
        if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }
        
        context.Response.StatusCode = problemDetails.Status.Value;
        context.Response.ContentType = "application/problem+json";
        
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}