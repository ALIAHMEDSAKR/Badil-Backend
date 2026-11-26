using Badil.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Badil.Application.Common.Exceptions;

namespace Badil.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
       HttpContext httpContext,
       Exception exception,
       CancellationToken cancellationToken)
    {
        //Console.WriteLine("HANDLER HIT: " + exception.GetType().Name);

        var response = exception switch
        {
            RequestValidationException validation => new ErrorResponse(
                Status: StatusCodes.Status400BadRequest,
                Title: "One or more validation errors occurred.",
                TraceId: httpContext.TraceIdentifier,
                Errors: validation.Errors),

            _ => new ErrorResponse(
                Status: StatusCodes.Status500InternalServerError,
                Title: "An unexpected error occurred.",
                TraceId: httpContext.TraceIdentifier)
        };

        if (response.Status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = response.Status;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

}