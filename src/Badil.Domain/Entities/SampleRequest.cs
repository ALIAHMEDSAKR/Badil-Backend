using Badil.Domain.Enums;
namespace Badil.Domain.Entities;

public sealed class SampleRequest : Entity
{
    public Guid OfferId { get; private set; }

    public Guid RequesterFactoryId { get; private set; }

    public decimal Quantity { get; private set; }

    public SampleStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private SampleRequest()
    {
    }

    public SampleRequest(
        Guid offerId,
        Guid requesterFactoryId,
        decimal quantity,
        DateTime createdAtUtc)
    {
        if (offerId == Guid.Empty ||
            requesterFactoryId == Guid.Empty)
        {
            throw new DomainException(
                "Offer and requester factory are required.");
        }

        if (quantity <= 0)
            throw new DomainException("Sample quantity must be positive.");

        OfferId = offerId;
        RequesterFactoryId = requesterFactoryId;
        Quantity = quantity;
        Status = SampleStatus.Requested;
        CreatedAtUtc = Utc(createdAtUtc);
    }

    public void Approve()
    {
        EnsureStatus(SampleStatus.Requested);
        Status = SampleStatus.Approved;
    }

    public void MarkSent()
    {
        EnsureStatus(SampleStatus.Approved);
        Status = SampleStatus.Sent;
    }

    public void Decline()
    {
        EnsureStatus(SampleStatus.Requested);
        Status = SampleStatus.Declined;
    }

    private void EnsureStatus(SampleStatus expected)
    {
        if (Status != expected)
            throw new DomainException(
                $"Sample request must be in {expected} status.");
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}