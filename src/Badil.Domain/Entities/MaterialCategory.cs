using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class MaterialCategory : Entity
{
    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    private MaterialCategory()
    {
    }

    public MaterialCategory(string name)
    {
        Name = Required(name);
        IsActive = true;
    }

    public void Rename(string name)
    {
        Name = Required(name);
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Category is already inactive.");

        IsActive = false;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Category is already active.");

        IsActive = true;
    }

    private static string Required(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Category name is required.");

        return value.Trim();
    }
}