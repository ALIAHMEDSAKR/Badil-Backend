using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class SavedSearch : Entity
{
    public Guid FactoryId { get; private set; }

    public Guid? CategoryId { get; private set; }

    public string? Keywords { get; private set; }

    public int RadiusKm { get; private set; }

    public bool IsActive { get; private set; }

    private SavedSearch()
    {
    }

    public SavedSearch(
        Guid factoryId,
        Guid? categoryId,
        string? keywords,
        int radiusKm)
    {
        if (factoryId == Guid.Empty)
            throw new DomainException("Factory ID is required.");

        if (categoryId == Guid.Empty)
            throw new DomainException("Category ID cannot be empty.");

        if (radiusKm < 0)
            throw new DomainException("Radius cannot be negative.");

        FactoryId = factoryId;
        CategoryId = categoryId;
        Keywords = Normalize(keywords);
        RadiusKm = radiusKm;
        IsActive = true;

        EnsureSearchCriteria();
    }

    public void Update(
        Guid? categoryId,
        string? keywords,
        int radiusKm)
    {
        if (categoryId == Guid.Empty)
            throw new DomainException("Category ID cannot be empty.");

        if (radiusKm < 0)
            throw new DomainException("Radius cannot be negative.");

        CategoryId = categoryId;
        Keywords = Normalize(keywords);
        RadiusKm = radiusKm;

        EnsureSearchCriteria();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private void EnsureSearchCriteria()
    {
        if (!CategoryId.HasValue && string.IsNullOrWhiteSpace(Keywords))
            throw new DomainException(
                "A category or search keyword is required.");
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}