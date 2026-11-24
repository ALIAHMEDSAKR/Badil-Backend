using Badil.Application.Features.Ping;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Badil.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    private readonly ISender _sender;

    public PingController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<PingResponse>> Get([FromQuery] string? name , CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new PingQuery(name), cancellationToken);
        return Ok(result);
    }
}