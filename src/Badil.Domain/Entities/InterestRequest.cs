
using Badil.Domain.Enums;

namespace Badil.Domain.Entities;

public class InterestRequest
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid ListingId { get; private set; }

    public Guid InterestedUserId { get; private set; }

    public string? Message { get; private set; }

    public InterestRequestStatus Status { get; private set; } =
        InterestRequestStatus.Pending;

    public DateTime CreatedAtUtc { get; private set; } =
        DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; private set; } =
        DateTime.UtcNow;

    public Listing? Listing { get; private set; }

    private InterestRequest()
    {
        // Used by Entity Framework Core.
    }

    public InterestRequest(
        Guid listingId,
        Guid interestedUserId,
        string? message = null)
    {
        if (listingId == Guid.Empty)
            throw new ArgumentException(
                "Listing ID is required.", nameof(listingId));

        if (interestedUserId == Guid.Empty)
            throw new ArgumentException(
                "Interested user ID is required.", nameof(interestedUserId));

        ListingId = listingId;
        InterestedUserId = interestedUserId;
        Message = string.IsNullOrWhiteSpace(message)
            ? null
            : message.Trim();
    }

    public void UpdateStatus(InterestRequestStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
