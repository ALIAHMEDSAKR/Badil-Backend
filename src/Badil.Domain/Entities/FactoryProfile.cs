namespace Badil.Domain.Entities;

public class FactoryProfile
{
    public Guid Id { get;private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Name { get;private set; } = string.Empty;
    public string? Description { get;private set; }
    public string Address { get; private set; } = string.Empty;
    public string City { get;private set; } = string.Empty;
    public string ContactPhone { get;private set; } = string.Empty;
    public DateTime CreatedAtUtc { get;private set; } = DateTime.UtcNow;
    private readonly List<Listing> _listings = new();
    public IReadOnlyCollection<Listing> Listings => _listings.AsReadOnly();
    
    private FactoryProfile() {/*Used by EF Core */ }
    public FactoryProfile(
        Guid userId,
        string name,
        string address,
        string city,
        string contactPhone,
        string? description = null
        )
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Factory Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required.", nameof(address));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));
        if (string.IsNullOrWhiteSpace(contactPhone))
            throw new ArgumentException("Contact phone is required.", nameof(contactPhone));

        UserId = userId;
        Name = name.Trim();
        Address = address.Trim();
        City = city.Trim();
        ContactPhone = contactPhone.Trim();
        Description = description?.Trim();
    }

        public void UpdateContactInfo(string address,
            string city,string contactPhone)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required.", nameof(address));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));

        if (string.IsNullOrWhiteSpace(contactPhone))
            throw new ArgumentException("Contact phone is required.", nameof(contactPhone));

        Address = address.Trim();
        City = city.Trim();
        ContactPhone = contactPhone.Trim();
    }

    }



