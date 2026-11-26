using Badil.Domain.Common;
using Badil.Domain.Enums;

namespace Badil.Domain.Entities;

public sealed class Notification : Entity
{
    public Guid UserId { get; private set; }

    public NotificationType Type { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Body { get; private set; } = string.Empty;

    public string RelatedEntityType { get; private set; } = string.Empty;

    public Guid RelatedEntityId { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Notification()
    {
    }

    public Notification(
        Guid userId,
        NotificationType type,
        string title,
        string body,
        string relatedEntityType,
        Guid relatedEntityId,
        DateTime createdAtUtc)
    {
        if (userId == Guid.Empty || relatedEntityId == Guid.Empty)
            throw new DomainException("User and related entity are required.");

        if (!Enum.IsDefined(type))
            throw new DomainException("Invalid notification type.");

        UserId = userId;
        Type = type;
        Title = Required(title, nameof(Title));
        Body = Required(body, nameof(Body));
        RelatedEntityType = Required(
            relatedEntityType,
            nameof(RelatedEntityType));

        RelatedEntityId = relatedEntityId;
        CreatedAtUtc = Utc(createdAtUtc);
        IsRead = false;
    }

    public void MarkRead()
    {
        IsRead = true;
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