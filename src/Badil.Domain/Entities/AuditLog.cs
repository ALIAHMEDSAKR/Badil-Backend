using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class AuditLog : Entity
{
    public Guid? ActorUserId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? EntityType { get; private set; }

    public Guid? EntityId { get; private set; }

    public string? Details { get; private set; }

    public string? IpAddress { get; private set; }

    public string CorrelationId { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    private AuditLog()
    {
    }

    public AuditLog(
        Guid? actorUserId,
        string action,
        string? entityType,
        Guid? entityId,
        string? details,
        string? ipAddress,
        string correlationId,
        DateTime createdAtUtc)
    {
        if (actorUserId == Guid.Empty)
            throw new DomainException("Actor ID cannot be empty.");

        if (entityId == Guid.Empty)
            throw new DomainException("Entity ID cannot be empty.");

        if (entityId.HasValue && string.IsNullOrWhiteSpace(entityType))
            throw new DomainException(
                "Entity type is required when entity ID is provided.");

        ActorUserId = actorUserId;
        Action = Required(action, nameof(Action));
        EntityType = Optional(entityType);
        EntityId = entityId;
        Details = Optional(details);
        IpAddress = Optional(ipAddress);
        CorrelationId = Required(correlationId, nameof(CorrelationId));
        CreatedAtUtc = Utc(createdAtUtc);
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

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}