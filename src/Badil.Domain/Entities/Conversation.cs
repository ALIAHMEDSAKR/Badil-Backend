using Badil.Domain.Common;
using Badil.Domain.Enums;

namespace Badil.Domain.Entities;

public sealed class Conversation : Entity
{
    public ListingType ListingType { get; private set; }

    public Guid? OfferId { get; private set; }

    public Guid? RequestId { get; private set; }

    public Guid InitiatorFactoryId { get; private set; }

    public Guid OwnerFactoryId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Conversation()
    {
    }

    public Conversation(
        ListingType listingType,
        Guid? offerId,
        Guid? requestId,
        Guid initiatorFactoryId,
        Guid ownerFactoryId,
        DateTime createdAtUtc)
    {
        if (initiatorFactoryId == Guid.Empty ||
            ownerFactoryId == Guid.Empty)
        {
            throw new DomainException("Both factories are required.");
        }

        if (initiatorFactoryId == ownerFactoryId)
            throw new DomainException(
                "A factory cannot start a conversation with itself.");

        if (listingType == ListingType.Offer)
        {
            if (!offerId.HasValue || requestId.HasValue)
                throw new DomainException(
                    "An Offer conversation must reference only an Offer.");
        }
        else if (listingType == ListingType.Request)
        {
            if (!requestId.HasValue || offerId.HasValue)
                throw new DomainException(
                    "A Request conversation must reference only a Request.");
        }
        else
        {
            throw new DomainException("Invalid listing type.");
        }

        if (offerId == Guid.Empty || requestId == Guid.Empty)
            throw new DomainException("Listing ID cannot be empty.");

        ListingType = listingType;
        OfferId = offerId;
        RequestId = requestId;
        InitiatorFactoryId = initiatorFactoryId;
        OwnerFactoryId = ownerFactoryId;
        CreatedAtUtc = Utc(createdAtUtc);
    }

    public Message Send(
        Guid senderFactoryId,
        string body,
        DateTime sentAtUtc)
    {
        if (senderFactoryId != InitiatorFactoryId &&
            senderFactoryId != OwnerFactoryId)
        {
            throw new DomainException(
                "Sender is not a participant in this conversation.");
        }

        return new Message(
            Id,
            senderFactoryId,
            body,
            sentAtUtc);
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}