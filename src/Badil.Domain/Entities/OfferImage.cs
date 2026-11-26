using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class OfferImage : Entity
{
    public Guid OfferId { get; private set; }

    public string StoredFileName { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    private OfferImage()
    {
    }

    public OfferImage(
        Guid offerId,
        string storedFileName,
        int sortOrder)
    {
        if (offerId == Guid.Empty)
            throw new DomainException("Offer ID is required.");

        if (sortOrder < 0)
            throw new DomainException("Sort order cannot be negative.");

        OfferId = offerId;
        StoredFileName = Required(storedFileName);
        SortOrder = sortOrder;
    }

    public void ChangeSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new DomainException("Sort order cannot be negative.");

        SortOrder = sortOrder;
    }

    private static string Required(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Stored file name is required.");

        return value.Trim();
    }
}