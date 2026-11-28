using Serilog.Core;
using Serilog.Events;

namespace Badil.Api.Logging;

/// <summary>
/// Ensures every formatted log line has a CorrelationId property. Request-scoped values
/// supplied by LogContext take precedence; non-request events use a visible placeholder.
/// </summary>
internal sealed class CorrelationIdFallbackEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", "-"));
    }
}
