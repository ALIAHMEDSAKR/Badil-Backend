using Badil.Application.Common.interfaces;

namespace Badil.Application.Features.Ping;

public record PingQuery(string? Name) : IQuery<PingResponse>;