using Badil.Domain.Common;
using Badil.Domain.Enums;
using Badil.Domain.ValueObjects;

namespace Badil.Domain.Entities;

public sealed class Offer : Entity
{
    public Guid FactoryId { get; private set; }

    public Guid CategoryId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Quantity { get; private set; }

    public decimal RemainingQuantity { get; private set; }

    public decimal MinOrderQuantity { get; private set; }

    public MeasurementUnit Unit { get; private set; }

    public decimal AskingUnitPrice { get; private set; }

    public GeoLocation Location { get; private set; } = null!;

    public OfferStatus Status { get; private set; }

    public LockReason? LockReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    // SQL Server concurrency token; EF mapping will configure it later.
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private Offer()
    {
    }

    public Offer(
        Guid factoryId,
        Guid categoryId,
        string title,
        string description,
        decimal quantity,
        decimal minOrderQuantity,
        MeasurementUnit unit,
        decimal askingUnitPrice,
        GeoLocation location,
        DateTime createdAtUtc)
    {
        if (factoryId == Guid.Empty)
            throw new DomainException("Factory ID is required.");

        if (categoryId == Guid.Empty)
            throw new DomainException("Category ID is required.");

        FactoryId = factoryId;
        CategoryId = categoryId;

        Title = Required(title, nameof(Title));
        Description = Required(description, nameof(Description));

        ValidateQuantity(quantity);
        ValidateMinimumOrder(minOrderQuantity, quantity);
        ValidatePrice(askingUnitPrice);

        Quantity = quantity;
        RemainingQuantity = quantity;
        MinOrderQuantity = minOrderQuantity;
        Unit = unit;
        AskingUnitPrice = askingUnitPrice;

        Location = location
            ?? throw new DomainException("Offer location is required.");

        CreatedAtUtc = Utc(createdAtUtc);
        UpdatedAtUtc = CreatedAtUtc;

        Status = OfferStatus.Available;
    }

    public void UpdateDetails(
        string title,
        string description,
        decimal minOrderQuantity,
        decimal askingUnitPrice,
        DateTime updatedAtUtc)
    {
        EnsureStatus(OfferStatus.Available);

        Title = Required(title, nameof(Title));
        Description = Required(description, nameof(Description));

        ValidateMinimumOrder(minOrderQuantity, RemainingQuantity);
        ValidatePrice(askingUnitPrice);

        MinOrderQuantity = minOrderQuantity;
        AskingUnitPrice = askingUnitPrice;
        UpdatedAtUtc = Utc(updatedAtUtc);
    }

    public void Reserve(decimal quantity, DateTime updatedAtUtc)
    {
        EnsureStatus(OfferStatus.Available);

        if (quantity <= 0)
            throw new DomainException("Reserved quantity must be positive.");

        if (quantity < MinOrderQuantity)
            throw new DomainException("Quantity is below the minimum order quantity.");

        if (quantity > RemainingQuantity)
            throw new DomainException("Not enough remaining quantity.");

        RemainingQuantity -= quantity;

        if (RemainingQuantity == 0)
            Status = OfferStatus.Reserved;

        UpdatedAtUtc = Utc(updatedAtUtc);
    }

    public void Release(decimal quantity, DateTime updatedAtUtc)
    {
        if (Status != OfferStatus.Available &&
            Status != OfferStatus.Reserved)
        {
            throw new DomainException(
                "Quantity cannot be released from the current offer status.");
        }

        if (quantity <= 0)
            throw new DomainException("Released quantity must be positive.");

        if (RemainingQuantity + quantity > Quantity)
            throw new DomainException("Released quantity exceeds the original quantity.");

        RemainingQuantity += quantity;

        if (RemainingQuantity > 0)
            Status = OfferStatus.Available;

        UpdatedAtUtc = Utc(updatedAtUtc);
    }

    public void MarkSold(DateTime updatedAtUtc)
    {
        var canBeSoldExternally =
            Status == OfferStatus.Available &&
            RemainingQuantity == Quantity;

        var allReservedDealsCompleted =
            Status == OfferStatus.Reserved &&
            RemainingQuantity == 0;

        if (!canBeSoldExternally && !allReservedDealsCompleted)
        {
            throw new DomainException(
                "Offer cannot be marked as sold in its current state.");
        }

        RemainingQuantity = 0;
        Status = OfferStatus.Sold;
        UpdatedAtUtc = Utc(updatedAtUtc);
    }

    public void Close(DateTime updatedAtUtc)
    {
        EnsureStatus(OfferStatus.Available);

        Status = OfferStatus.Closed;
        UpdatedAtUtc = Utc(updatedAtUtc);
    }

    public void Lock(LockReason reason, DateTime updatedAtUtc)
    {
        EnsureStatus(OfferStatus.Available);

        if (!Enum.IsDefined(reason))
            throw new DomainException("Invalid lock reason.");

        LockReason = reason;
        Status = OfferStatus.Locked;
        UpdatedAtUtc = Utc(updatedAtUtc);
    }

    public void Unlock(DateTime updatedAtUtc)
    {
        EnsureStatus(OfferStatus.Locked);

        LockReason = null;
        Status = OfferStatus.Available;
        UpdatedAtUtc = Utc(updatedAtUtc);
    }

    private void EnsureStatus(OfferStatus expected)
    {
        if (Status != expected)
            throw new DomainException($"Offer must be in {expected} status.");
    }

    private static void ValidateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Offer quantity must be greater than zero.");
    }

    private static void ValidateMinimumOrder(decimal minimum, decimal maximum)
    {
        if (minimum < 0 || minimum > maximum)
            throw new DomainException(
                "Minimum order must be between zero and the available quantity.");
    }

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("Asking price cannot be negative.");
    }

    private static string Required(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{field} is required.");

        return value.Trim();
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}