namespace Badil.Api.Models;

public record ErrorResponse(
    int Status,
    string Title,
    string? TraceId = null,
    IDictionary<string,string[]>? Errors = null
);