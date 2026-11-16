using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class RefreshToken : Entity
{
    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public string? ReplacedByHash { get; private set; }

    public string? CreatedByIp { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public bool IsRevoked => RevokedAtUtc.HasValue;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    private RefreshToken()
    {
    }

    public RefreshToken(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc,
        DateTime createdAtUtc,
        string? createdByIp = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID is required.");

        UserId = userId;
        TokenHash = Required(tokenHash, nameof(TokenHash));

        CreatedAtUtc = Utc(createdAtUtc, nameof(CreatedAtUtc));
        ExpiresAtUtc = Utc(expiresAtUtc, nameof(ExpiresAtUtc));

        if (ExpiresAtUtc <= CreatedAtUtc)
            throw new DomainException("Token expiration must be after creation.");

        CreatedByIp = Optional(createdByIp);
    }

    public void Revoke(
        DateTime revokedAtUtc,
        string? replacedByHash = null)
    {
        if (IsRevoked)
            throw new DomainException("Token is already revoked.");

        RevokedAtUtc = Utc(revokedAtUtc, nameof(RevokedAtUtc));

        if (RevokedAtUtc < CreatedAtUtc)
            throw new DomainException("Revocation date cannot be before creation.");

        ReplacedByHash = Optional(replacedByHash);
    }

    private static string Required(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{field} is required.");

        return value.Trim();
    }

    private static string? Optional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static DateTime Utc(DateTime value, string field)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException($"{field} must be UTC.");

        return value;
    }
}