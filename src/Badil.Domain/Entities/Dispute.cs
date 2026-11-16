using Badil.Domain.Common;
using Badil.Domain.Enums;

namespace Badil.Domain.Entities;

public sealed class Dispute : Entity
{
    public Guid DealId { get; private set; }

    public Guid RaisedByFactoryId { get; private set; }

    public string Reason { get; private set; } = string.Empty;

    public DisputeStatus Status { get; private set; }

    public DisputeResolution? Resolution { get; private set; }

    public Guid? ResolvedByUserId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ResolvedAtUtc { get; private set; }

    private Dispute()
    {
    }

    public Dispute(
        Guid dealId,
        Guid raisedByFactoryId,
        string reason,
        DateTime createdAtUtc)
    {
        if (dealId == Guid.Empty ||
            raisedByFactoryId == Guid.Empty)
        {
            throw new DomainException(
                "Deal and factory are required.");
        }

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Dispute reason is required.");

        DealId = dealId;
        RaisedByFactoryId = raisedByFactoryId;
        Reason = reason.Trim();
        Status = DisputeStatus.Open;
        CreatedAtUtc = Utc(createdAtUtc);
    }

    public void Resolve(
        Guid adminUserId,
        DisputeResolution resolution,
        DateTime resolvedAtUtc)
    {
        if (Status != DisputeStatus.Open)
            throw new DomainException("Dispute is not open.");

        if (adminUserId == Guid.Empty)
            throw new DomainException("Admin user is required.");

        if (!Enum.IsDefined(resolution))
            throw new DomainException("Invalid dispute resolution.");

        ResolvedByUserId = adminUserId;
        Resolution = resolution;
        ResolvedAtUtc = Utc(resolvedAtUtc);
        Status = DisputeStatus.Resolved;
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}