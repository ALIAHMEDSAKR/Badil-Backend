using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class PlatformSetting
{
    public string SettingKey { get; private set; } = string.Empty;

    public string Value { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public Guid? UpdatedByUserId { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    private PlatformSetting()
    {
    }

    public PlatformSetting(
        string settingKey,
        string value,
        string description,
        Guid? updatedByUserId,
        DateTime updatedAtUtc)
    {
        SettingKey = Required(settingKey, nameof(SettingKey));
        Value = Required(value, nameof(Value));
        Description = Required(description, nameof(Description));

        if (updatedByUserId == Guid.Empty)
            throw new DomainException("Admin ID cannot be empty.");

        UpdatedByUserId = updatedByUserId;
        UpdatedAtUtc = Utc(updatedAtUtc);
    }

    public void Change(
        string value,
        Guid adminUserId,
        DateTime updatedAtUtc)
    {
        if (adminUserId == Guid.Empty)
            throw new DomainException("Admin user ID is required.");

        Value = Required(value, nameof(Value));
        UpdatedByUserId = adminUserId;
        UpdatedAtUtc = Utc(updatedAtUtc);
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