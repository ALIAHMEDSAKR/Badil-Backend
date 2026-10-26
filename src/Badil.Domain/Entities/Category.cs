namespace Badil.Domain.Entities;

public class Category
{
    public Guid Id { get;private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public Category? ParentCategory { get; private set; }
    private readonly List<Category> _subCategories = new();
    public IReadOnlyCollection<Category> SubCategories=>_subCategories.AsReadOnly();
    private Category() { }

    public Category( string name , 
        string?description=null,
        Guid? parentCategoryId=null
       )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        if (parentCategoryId == Guid.Empty)
            throw new ArgumentException(
                "Parent category ID cannot be empty.",
                nameof(parentCategoryId));

        Name = name.Trim();
        Description = description?.Trim();
        ParentCategoryId = parentCategoryId;
    }
    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
    }
    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
