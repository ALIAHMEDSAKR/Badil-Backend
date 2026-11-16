using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class AdminProfile
{
    // Shared primary key with the Identity user
    public Guid UserId { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public bool IsOwner { get; private set; }

    public bool MustChangePassword { get; private set; }

    // Null for the initially seeded owner admin
    public Guid? InvitedByUserId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private AdminProfile()
    {
    }

    public AdminProfile(
        Guid userId,
        string fullName,
        bool isOwner,
        bool mustChangePassword,
        Guid? invitedByUserId,
        DateTime createdAtUtc)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Admin user ID is required.");

        if (invitedByUserId == Guid.Empty)
            throw new DomainException("Inviter ID cannot be empty.");

        if (invitedByUserId == userId)
            throw new DomainException("An admin cannot invite himself.");

        if (isOwner && invitedByUserId.HasValue)
            throw new DomainException("The owner admin cannot have an inviter.");

        UserId = userId;
        FullName = Required(fullName, nameof(FullName));
        IsOwner = isOwner;
        MustChangePassword = mustChangePassword;
        InvitedByUserId = invitedByUserId;
        CreatedAtUtc = Utc(createdAtUtc, nameof(CreatedAtUtc));
    }

    public void UpdateFullName(string fullName)
    {
        FullName = Required(fullName, nameof(FullName));
    }

    public void MarkPasswordChanged()
    {
        MustChangePassword = false;
    }

    private static string Required(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{field} is required.");

        return value.Trim();
    }

    private static DateTime Utc(DateTime value, string field)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException($"{field} must be UTC.");

        return value;
    }
}