using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class Match : Entity
{
    public Guid OfferId { get; private set; }

    public Guid RequestId { get; private set; }

    public decimal DistanceKm { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Match()
    {
    }

    public Match(
        Guid offerId,
        Guid requestId,
        decimal distanceKm,
        DateTime createdAtUtc)
    {
        if (offerId == Guid.Empty || requestId == Guid.Empty)
            throw new DomainException("Offer and request are required.");

        if (offerId == requestId)
            throw new DomainException("Offer and request IDs must differ.");

        if (distanceKm < 0)
            throw new DomainException("Distance cannot be negative.");

        OfferId = offerId;
        RequestId = requestId;
        DistanceKm = distanceKm;
        CreatedAtUtc = Utc(createdAtUtc);
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}