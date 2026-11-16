
namespace Badil.Domain.Entities;

public sealed class FactoryProfile : Entity
{
    // Account relationship
    public Guid UserId { get; private set; }

    // Basic factory information
    public string Name { get; private set; } = string.Empty;

    public string Sector { get; private set; } = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public string? LogoFileName { get; private set; }

    public GeoLocation Location { get; private set; } = null!;

    // Verification information
    public string? CommercialRegisterNumber { get; private set; }

    public string? TaxId { get; private set; }

    public VerificationStatus VerificationStatus { get; private set; }
        = VerificationStatus.NotSubmitted;

    public string? VerificationRejectionReason { get; private set; }

    public DateTime? VerificationSubmittedAtUtc { get; private set; }

    public Guid? VerificationReviewedByUserId { get; private set; }

    public DateTime? VerificationReviewedAtUtc { get; private set; }

    // Creation date
    public DateTime CreatedAtUtc { get; private set; }

    // Private constructor for EF Core materialization
    private FactoryProfile()
    {
    }

    public FactoryProfile(
        Guid userId,
        string name,
        string sector,
        string address,
        string city,
        GeoLocation location,
        DateTime createdAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(
                "Factory must be associated with a valid user.");
        }

        UserId = userId;

        Name = NormalizeRequired(name, nameof(Name));

        Sector = NormalizeRequired(sector, nameof(Sector));

        Address = NormalizeRequired(address, nameof(Address));

        City = NormalizeRequired(city, nameof(City));

        Location = location
            ?? throw new DomainException("Factory location is required.");

        CreatedAtUtc = EnsureUtc(
            createdAtUtc,
            nameof(CreatedAtUtc));
    }

    // Update basic factory information
    public void UpdateProfileDetails(
        string name,
        string sector,
        string address,
        string city,
        GeoLocation location)
    {
        Name = NormalizeRequired(name, nameof(Name));

        Sector = NormalizeRequired(sector, nameof(Sector));

        Address = NormalizeRequired(address, nameof(Address));

        City = NormalizeRequired(city, nameof(City));

        Location = location
            ?? throw new DomainException("Factory location is required.");
    }

    // Update the optional factory logo file name
    public void UpdateLogo(string? logoFileName)
    {
        LogoFileName = NormalizeOptional(logoFileName);
    }

    // Update legal verification information
    public void UpdateVerificationDetails(
        string commercialRegisterNumber,
        string taxId)
    {
        if (VerificationStatus == VerificationStatus.Pending)
        {
            throw new DomainException(
                "Verification details cannot be changed while review is pending.");
        }

        if (VerificationStatus == VerificationStatus.Approved)
        {
            throw new DomainException(
                "Approved verification details cannot be changed directly.");
        }

        CommercialRegisterNumber = NormalizeRequired(
            commercialRegisterNumber,
            nameof(CommercialRegisterNumber));

        TaxId = NormalizeRequired(
            taxId,
            nameof(TaxId));
    }

    // Submit factory verification
    public void SubmitForVerification(DateTime submittedAtUtc)
    {
        if (VerificationStatus != VerificationStatus.NotSubmitted &&
            VerificationStatus != VerificationStatus.Rejected)
        {
            throw new DomainException(
                "Factory verification can only be submitted from NotSubmitted or Rejected status.");
        }

        if (string.IsNullOrWhiteSpace(CommercialRegisterNumber))
        {
            throw new DomainException(
                "Commercial register number is required.");
        }

        if (string.IsNullOrWhiteSpace(TaxId))
        {
            throw new DomainException(
                "Tax ID is required.");
        }

        VerificationSubmittedAtUtc = EnsureUtc(
            submittedAtUtc,
            nameof(VerificationSubmittedAtUtc));

        VerificationStatus = VerificationStatus.Pending;

        VerificationRejectionReason = null;

        VerificationReviewedByUserId = null;

        VerificationReviewedAtUtc = null;
    }

    // Approve factory verification
    public void Approve(
        Guid adminUserId,
        DateTime reviewedAtUtc)
    {
        EnsurePendingVerification();

        if (adminUserId == Guid.Empty)
        {
            throw new DomainException(
                "A valid admin user is required to approve verification.");
        }

        VerificationStatus = VerificationStatus.Approved;

        VerificationReviewedByUserId = adminUserId;

        VerificationReviewedAtUtc = EnsureUtc(
            reviewedAtUtc,
            nameof(VerificationReviewedAtUtc));

        VerificationRejectionReason = null;
    }

    // Reject factory verification
    public void Reject(
        Guid adminUserId,
        string reason,
        DateTime reviewedAtUtc)
    {
        EnsurePendingVerification();

        if (adminUserId == Guid.Empty)
        {
            throw new DomainException(
                "A valid admin user is required to reject verification.");
        }

        VerificationRejectionReason = NormalizeRequired(
            reason,
            nameof(VerificationRejectionReason));

        VerificationStatus = VerificationStatus.Rejected;

        VerificationReviewedByUserId = adminUserId;

        VerificationReviewedAtUtc = EnsureUtc(
            reviewedAtUtc,
            nameof(VerificationReviewedAtUtc));
    }

    private void EnsurePendingVerification()
    {
        if (VerificationStatus != VerificationStatus.Pending)
        {
            throw new DomainException(
                "Only pending factory verifications can be reviewed.");
        }
    }

    private static string NormalizeRequired(
        string value,
        string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                $"{propertyName} is required.");
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static DateTime EnsureUtc(
        DateTime value,
        string propertyName)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new DomainException(
                $"{propertyName} must be in UTC.");
        }

        return value;
    }
}
