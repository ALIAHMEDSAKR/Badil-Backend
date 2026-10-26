
using Badil.Domain.Enums;

namespace Badil.Domain.Entities;

public class Listing
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid FactoryProfileId { get; private set; }

    public Guid CategoryId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Quantity { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public PricingMode PricingMode { get; private set; }

    public decimal? PricePerUnit { get; private set; }

    public string Currency { get; private set; } = "EGP";

    public string? Condition { get; private set; }

    public ListingStatus Status { get; private set; } = ListingStatus.Draft;

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? PublishedAtUtc { get; private set; }

    public FactoryProfile? FactoryProfile { get; private set; }

    public Category? Category { get; private set; }

    private readonly List<ListingImage> _images = new();

    public IReadOnlyCollection<ListingImage> Images => _images.AsReadOnly();

    private Listing()
    {
        // Used by Entity Framework Core.
    }

    public Listing(
        Guid factoryProfileId,
        Guid categoryId,
        string title,
        string description,
        decimal quantity,
        string unit,
        PricingMode pricingMode,
        decimal? pricePerUnit,
        string currency = "EGP",
        string? condition = null)
    {
        if (factoryProfileId == Guid.Empty)
            throw new ArgumentException(
                "Factory profile is required.", nameof(factoryProfileId));

        if (categoryId == Guid.Empty)
            throw new ArgumentException(
                "Category is required.", nameof(categoryId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Description is required.", nameof(description));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity), "Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException(
                "Unit is required.", nameof(unit));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException(
                "Currency is required.", nameof(currency));

        ValidatePricing(pricingMode, pricePerUnit);

        FactoryProfileId = factoryProfileId;
        CategoryId = categoryId;
        Title = title.Trim();
        Description = description.Trim();
        Quantity = quantity;
        Unit = unit.Trim();
        PricingMode = pricingMode;
        PricePerUnit = pricePerUnit;
        Currency = currency.Trim().ToUpperInvariant();
        Condition = condition?.Trim();
    }

    public void UpdateDetails(
        Guid categoryId,
        string title,
        string description,
        decimal quantity,
        string unit,
        PricingMode pricingMode,
        decimal? pricePerUnit,
        string currency,
        string? condition)
    {
        if (Status == ListingStatus.Archived)
            throw new InvalidOperationException(
                "Archived listings cannot be edited.");

        if (categoryId == Guid.Empty)
            throw new ArgumentException(
                "Category is required.", nameof(categoryId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Description is required.", nameof(description));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity), "Quantity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException(
                "Unit is required.", nameof(unit));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException(
                "Currency is required.", nameof(currency));

        ValidatePricing(pricingMode, pricePerUnit);

        CategoryId = categoryId;
        Title = title.Trim();
        Description = description.Trim();
        Quantity = quantity;
        Unit = unit.Trim();
        PricingMode = pricingMode;
        PricePerUnit = pricePerUnit;
        Currency = currency.Trim().ToUpperInvariant();
        Condition = condition?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status == ListingStatus.Archived)
            throw new InvalidOperationException(
                "Archived listings cannot be published.");

        Status = ListingStatus.Published;
        PublishedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = ListingStatus.Archived;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static void ValidatePricing(
        PricingMode pricingMode,
        decimal? pricePerUnit)
    {
        if (!Enum.IsDefined(pricingMode))
            throw new ArgumentOutOfRangeException(nameof(pricingMode));

        if (pricePerUnit < 0)
            throw new ArgumentOutOfRangeException(
                nameof(pricePerUnit), "Price cannot be negative.");

        if (pricingMode == PricingMode.Fixed && pricePerUnit is null)
            throw new ArgumentException(
                "A fixed-price listing requires a price.",
                nameof(pricePerUnit));
    }
}
