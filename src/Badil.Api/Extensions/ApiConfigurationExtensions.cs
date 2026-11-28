namespace Badil.Api.Extensions;

public static class ApiConfigurationExtensions
{
    internal const string CorsPolicyName = "ReactDevelopment";

    private const string LogOutputTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}";

    public static WebApplicationBuilder AddBadilApiConfiguration(this WebApplicationBuilder builder)
    {
        // Layer-specific Dependency Injection extensions 
        builder.Services.AddApplication ( );
        builder.Services.AddInfrastructure ( builder.Configuration );
        builder.Services.AddProblemDetails ( );
        builder.Services.AddExceptionHandler<GlobalExceptionHandler> ( );

        builder.Services.AddControllers ( );
        builder.Services.AddOpenApi ( );



        ArgumentNullException.ThrowIfNull(builder);

        ConfigureSerilog(builder);
        ConfigureCors(builder);

        // IStartupFilter lets us add middleware without adding more lines to Program.cs.
        builder.Services.AddTransient<IStartupFilter, ApiPipelineStartupFilter>();

        return builder;
    }

    private static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((_, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // A fallback makes the CorrelationId column visible for startup/background logs too.
                // HTTP request logs receive their actual value from CorrelationIdMiddleware.
                .Enrich.With(new CorrelationIdFallbackEnricher())
                .WriteTo.Console(outputTemplate: LogOutputTemplate)
                .WriteTo.File(
                    path: "logs/badil-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: LogOutputTemplate);
        });
    }

    private static void ConfigureCors(WebApplicationBuilder builder)
    {
        // Keep this list in configuration. It is intentionally empty until the React dev
        // server origin is known; an empty allow-list does not permit cross-origin access.
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .GetChildren()
            .Select(section => section.Value)
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(CorrelationIdMiddleware.HeaderName);

                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins);
                }
            });
        });
    }
 
}

internal sealed class ApiPipelineStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        ArgumentNullException.ThrowIfNull(next);

        return app =>
        {
            // Keep this order: the request logger must execute inside the CorrelationId scope.
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseSerilogRequestLogging(options =>
            {
                // Explicitly enrich the request-completion event as well as ordinary LogContext events.
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    if (httpContext.Items.TryGetValue(CorrelationIdMiddleware.HttpContextItemKey, out var value)
                        && value is string correlationId)
                    {
                        diagnosticContext.Set("CorrelationId", correlationId);
                    }
                };
            });
            app.UseCors(ApiConfigurationExtensions.CorsPolicyName);

            next(app);
        };
    }
}
