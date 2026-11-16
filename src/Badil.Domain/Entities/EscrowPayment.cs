using Badil.Domain.Common;
using Badil.Domain.Enums;

namespace Badil.Domain.Entities;

public sealed class EscrowPayment : Entity
{
    public Guid DealId { get; private set; }

    public decimal Amount { get; private set; }

    public EscrowStatus Status { get; private set; }

    public DateTime? HeldAtUtc { get; private set; }

    public DateTime? SettledAtUtc { get; private set; }

    private EscrowPayment()
    {
    }

    public EscrowPayment(
        Guid dealId,
        decimal amount)
    {
        if (dealId == Guid.Empty)
            throw new DomainException("Deal ID is required.");

        if (amount < 0)
            throw new DomainException("Escrow amount cannot be negative.");

        DealId = dealId;
        Amount = amount;
        Status = EscrowStatus.Pending;
    }

    public void Hold(DateTime heldAtUtc)
    {
        EnsureStatus(EscrowStatus.Pending);

        HeldAtUtc = Utc(heldAtUtc);
        Status = EscrowStatus.Held;
    }

    public void Release(DateTime settledAtUtc)
    {
        EnsureStatus(EscrowStatus.Held);

        SettledAtUtc = Utc(settledAtUtc);
        Status = EscrowStatus.Released;
    }

    public void Refund(DateTime settledAtUtc)
    {
        EnsureStatus(EscrowStatus.Held);

        SettledAtUtc = Utc(settledAtUtc);
        Status = EscrowStatus.Refunded;
    }

    private void EnsureStatus(EscrowStatus expected)
    {
        if (Status != expected)
            throw new DomainException(
                $"Escrow must be in {expected} status.");
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}