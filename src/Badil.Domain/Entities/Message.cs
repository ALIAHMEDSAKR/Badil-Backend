using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class Message : Entity
{
    public Guid ConversationId { get; private set; }

    public Guid SenderFactoryId { get; private set; }

    public string Body { get; private set; } = string.Empty;

    public DateTime SentAtUtc { get; private set; }

    public DateTime? ReadAtUtc { get; private set; }

    private Message()
    {
    }

    public Message(
        Guid conversationId,
        Guid senderFactoryId,
        string body,
        DateTime sentAtUtc)
    {
        if (conversationId == Guid.Empty ||
            senderFactoryId == Guid.Empty)
        {
            throw new DomainException(
                "Conversation and sender are required.");
        }

        if (string.IsNullOrWhiteSpace(body))
            throw new DomainException("Message body is required.");

        ConversationId = conversationId;
        SenderFactoryId = senderFactoryId;
        Body = body.Trim();
        SentAtUtc = Utc(sentAtUtc);
    }

    public void MarkAsRead(DateTime readAtUtc)
    {
        if (ReadAtUtc.HasValue)
            return;

        var timestamp = Utc(readAtUtc);

        if (timestamp < SentAtUtc)
            throw new DomainException(
                "Read time cannot be before sent time.");

        ReadAtUtc = timestamp;
    }

    private static DateTime Utc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException("Date must be UTC.");

        return value;
    }
}