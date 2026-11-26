using Badil.Domain.Common;
using Badil.Domain.Enums;

namespace Badil.Domain.Entities;

public sealed class Deal : Entity
{
    public Guid OfferId { get; private set; }

    public Guid? RequestId { get; private set; }

    public Guid SellerFactoryId { get; private set; }

    public Guid BuyerFactoryId { get; private set; }

    public decimal Quantity { get; private set; }

    public MeasurementUnit Unit { get; private set; }

    public decimal AgreedUnitPrice { get; private set; }

    public decimal TotalAmount { get; private set; }

    public decimal DistanceKm { get; private set; }

    public decimal TransportEstimate { get; private set; }

    public DealStatus Status { get; private set; }

    public DateTime? InspectionDeadlineUtc { get; private set; }

    public bool IsUnderAdminReview { get; private set; }

    public string? AdminReviewReason { get; private set; }

    public string? CancelReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private Deal()
    {
    }

    public Deal(
        Guid offerId,
        Guid? requestId,
        Guid sellerFactoryId,
        Guid buyerFactoryId,
        decimal quantity,
        MeasurementUnit unit,
        decimal agreedUnitPrice,
        decimal distanceKm,
        decimal transportEstimate,
        DateTime createdAtUtc)
    {
        if (offerId == Guid.Empty)
            throw new DomainException("Offer ID is required.");

        if (requestId == Guid.Empty)
            throw new DomainException("Request ID cannot be empty.");

        if (sellerFactoryId == Guid.Empty ||
            buyerFactoryId == Guid.Empty)
        {
            throw new DomainException("Both factories are required.");
        }

        if (sellerFactoryId == buyerFactoryId)
            throw new DomainException(
                "A factory cannot trade with itself.");

        if (quantity <= 0)
            throw new DomainException("Deal quantity must be positive.");

        if (agreedUnitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        if (distanceKm < 0 || transportEstimate < 0)
            throw new DomainException(
                "Distance and transport estimate cannot be negative.");

        OfferId = offerId;
        RequestId = requestId;
        SellerFactoryId = sellerFactoryId;
        BuyerFactoryId = buyerFactoryId;
        Quantity = quantity;
        Unit = unit;
        AgreedUnitPrice = agreedUnitPrice;

        // Calculated on the server, never trusted from the client.
        TotalAmount = quantity * agreedUnitPrice;

        DistanceKm = distanceKm;
        TransportEstimate = transportEstimate;

        Status = DealStatus.Proposed;
        CreatedAtUtc = Utc(createdAtUtc);
    }

    public void Accept()
    {
        EnsureNotUnderAdminReview();
        EnsureStatus(DealStatus.Proposed);

        Status = DealStatus.Accepted;
    }

    public void MarkEscrowFunded()
    {
        EnsureNotUnderAdminReview();
        EnsureStatus(DealStatus.Accepted);

        Status = DealStatus.EscrowFunded;
    }

    public void MarkShipped()
    {
        EnsureNotUnderAdminReview();
        EnsureStatus(DealStatus.EscrowFunded);

        Status = DealStatus.Shipped;
    }

    public void ConfirmDelivery(
        DateTime inspectionDeadlineUtc)
    {
        EnsureNotUnderAdminReview();
        EnsureStatus(DealStatus.Shipped);

        InspectionDeadlineUtc = Utc(inspectionDeadlineUtc);
        Status = DealStatus.InInspection;
    }

    public void AcceptInspection(DateTime completedAtUtc)
    {
        EnsureNotUnderAdminReview();
        EnsureStatus(DealStatus.InInspection);

        CompletedAtUtc = Utc(completedAtUtc);
        Status = DealStatus.Completed;
    }

    public void RaiseDispute(string reason)
    {
        EnsureNotUnderAdminReview();
        EnsureStatus(DealStatus.InInspection);

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Dispute reason is required.");

        Status = DealStatus.Disputed;
    }

    public void ResolveDispute(
        DisputeResolution resolution,
        DateTime resolvedAtUtc)
    {
        EnsureStatus(DealStatus.Disputed);

        if (resolution == DisputeResolution.ReleaseToSeller)
        {
            Status = DealStatus.Completed;
            CompletedAtUtc = Utc(resolvedAtUtc);
        }
        else if (resolution == DisputeResolution.RefundBuyer)
        {
            Status = DealStatus.Refunded;
        }
        else
        {
            throw new DomainException("Invalid dispute resolution.");
        }
    }

    public void Refund()
    {
        if (Status != DealStatus.EscrowFunded &&
            Status != DealStatus.Shipped &&
            Status != DealStatus.InInspection &&
            Status != DealStatus.Disputed)
        {
            throw new DomainException(
                "Deal cannot be refunded in its current status.");
        }

        Status = DealStatus.Refunded;
    }

    public void Cancel(string reason)
    {
        EnsureNotUnderAdminReview();

        if (Status != DealStatus.Proposed &&
            Status != DealStatus.Accepted)
        {
            throw new DomainException(
                "Only unfunded deals can be cancelled directly.");
        }

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required.");

        CancelReason = reason.Trim();
        Status = DealStatus.Cancelled;
    }

    public void FlagForAdminReview(string reason)
    {
        if (Status == DealStatus.Completed ||
            Status == DealStatus.Refunded ||
            Status == DealStatus.Cancelled)
        {
            throw new DomainException(
                "A final deal cannot be flagged for review.");
        }

        if (IsUnderAdminReview)
            throw new DomainException("Deal is already under review.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Review reason is required.");

        IsUnderAdminReview = true;
        AdminReviewReason = reason.Trim();
    }

    public void ClearAdminReview()
    {
        if (!IsUnderAdminReview)
            throw new DomainException(
                "Deal is not under admin review.");

        IsUnderAdminReview = false;
        AdminReviewReason = null;
    }

    private void EnsureNotUnderAdminReview()
    {
        if (IsUnderAdminReview)
            throw new DomainException(
                "Only an admin can change a deal under review.");
    }

    private void EnsureStatus(DealStatus expected)
    {
        if (Status != expected)
            throw new DomainException(
                $"Deal must be in {expected} status.");
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}