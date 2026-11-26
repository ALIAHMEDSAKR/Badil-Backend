using Badil.Application.Common.Interfaces;
using Badil.Domain;

namespace Badil.Infrastructure.Services;

public class pingService : IPingService
{
    public Pong GetPong()
    {
        return new Pong("Pong", DateTime.UtcNow);
    }
}