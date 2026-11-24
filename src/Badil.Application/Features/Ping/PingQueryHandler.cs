using MediatR;

namespace Badil.Application.Features.Ping;

public class PingQueryHandler : IRequestHandler<PingQuery , PingResponse> {

    private readonly IPingService _pingService;

    public PingQueryHandler(IPingService pingService)
    {
        _pingService = pingService;
    }

    public Task<PingResponse> Handle(PingQuery Request , CancellationToken ct) {
        var pong = _pingService.GetPong();
        return Task.FromResult(new PingResponse(pong.Message , pong.UtcNow));
    }
}