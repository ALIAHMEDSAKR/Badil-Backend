using Badil.Domain.Common;
using Badil.Domain.Enums;
using Badil.Domain.ValueObjects;

namespace Badil.Domain.Entities;

public sealed class Request : Entity
{
    public Guid FactoryId { get; private set; }

    public Guid CategoryId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public decimal Quantity { get; private set; }

    public MeasurementUnit Unit { get; private set; }

    public GeoLocation PreferredLocation { get; private set; } = null!;

    public int RadiusKm { get; private set; }

    public RequestStatus Status { get; private set; }

    public LockReason? LockReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Request()
    {
    }

    public Request(
        Guid factoryId,
        Guid categoryId,
        string description,
        decimal quantity,
        MeasurementUnit unit,
        GeoLocation preferredLocation,
        int radiusKm,
        DateTime createdAtUtc)
    {
        if (factoryId == Guid.Empty || categoryId == Guid.Empty)
            throw new DomainException("Factory and category are required.");

        if (quantity <= 0)
            throw new DomainException("Requested quantity must be positive.");

        if (radiusKm < 0)
            throw new DomainException("Radius cannot be negative.");

        FactoryId = factoryId;
        CategoryId = categoryId;
        Description = Required(description);
        Quantity = quantity;
        Unit = unit;

        PreferredLocation = preferredLocation
            ?? throw new DomainException("Preferred location is required.");

        RadiusKm = radiusKm;
        Status = RequestStatus.Open;
        CreatedAtUtc = Utc(createdAtUtc);
    }

    public void UpdateDetails(
        string description,
        decimal quantity,
        MeasurementUnit unit,
        GeoLocation preferredLocation,
        int radiusKm)
    {
        EnsureStatus(RequestStatus.Open);

        if (quantity <= 0)
            throw new DomainException("Requested quantity must be positive.");

        if (radiusKm < 0)
            throw new DomainException("Radius cannot be negative.");

        Description = Required(description);
        Quantity = quantity;
        Unit = unit;
        PreferredLocation = preferredLocation
            ?? throw new DomainException("Preferred location is required.");
        RadiusKm = radiusKm;
    }

    public void Fulfil()
    {
        EnsureStatus(RequestStatus.Open);
        Status = RequestStatus.Fulfilled;
    }

    public void Close()
    {
        EnsureStatus(RequestStatus.Open);
        Status = RequestStatus.Closed;
    }

    public void Lock(LockReason reason)
    {
        EnsureStatus(RequestStatus.Open);

        if (!Enum.IsDefined(reason))
            throw new DomainException("Invalid lock reason.");

        LockReason = reason;
        Status = RequestStatus.Locked;
    }

    public void Unlock()
    {
        EnsureStatus(RequestStatus.Locked);

        LockReason = null;
        Status = RequestStatus.Open;
    }

    private void EnsureStatus(RequestStatus expected)
    {
        if (Status != expected)
            throw new DomainException($"Request must be in {expected} status.");
    }

    private static string Required(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Description is required.");

        return value.Trim();
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}