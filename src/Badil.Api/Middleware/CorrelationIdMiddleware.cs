using Serilog.Context;

namespace Badil.Api.Middleware;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";
    public const string HttpContextItemKey = "Badil.CorrelationId";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var correlationId = GetCorrelationId(context);
        context.Items[HttpContextItemKey] = correlationId;

        // Make the ID available to the response and re-apply it immediately before headers
        // are sent, so downstream middleware cannot accidentally remove it.
        context.Response.Headers[HeaderName] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // All Serilog events emitted while this request is executing inherit this property.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var values))
        {
            var suppliedValue = values.FirstOrDefault()?.Trim();

            if (!string.IsNullOrWhiteSpace(suppliedValue))
            {
                return suppliedValue;
            }
        }

        return Guid.NewGuid().ToString("D");
    }
}
