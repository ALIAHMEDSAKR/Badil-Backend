namespace Badil.Domain.Entities;

public class ListingImage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ListingId { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public Listing? Listing { get; private set; }
    private ListingImage() { }
    public ListingImage(
        Guid listingId,
        string imageUrl,
        int sortOrder,
        bool isPrimary = false)
    {
        if (listingId == Guid.Empty)
            throw new ArgumentException(
                "Listing ID is required.", nameof(listingId));

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException(
                "Image URL is required.", nameof(imageUrl));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder), "Sort order cannot be negative.");

        ListingId = listingId;
        ImageUrl = imageUrl.Trim();
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
    }
    public void UpdateDisplayOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder), "Sort order cannot be negative.");

        SortOrder = sortOrder;
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    public void RemovePrimaryStatus()
    {
        IsPrimary = false;
    }
}
