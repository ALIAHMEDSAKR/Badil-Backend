using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class VerificationDocument : Entity
{
    public Guid FactoryId { get; private set; }

    public string DocType { get; private set; } = string.Empty;

    public string StoredFileName { get; private set; } = string.Empty;

    public DateTime UploadedAtUtc { get; private set; }

    private VerificationDocument()
    {
    }

    public VerificationDocument(
        Guid factoryId,
        string docType,
        string storedFileName,
        DateTime uploadedAtUtc)
    {
        if (factoryId == Guid.Empty)
            throw new DomainException("Factory ID is required.");

        FactoryId = factoryId;
        DocType = Required(docType, nameof(DocType));
        StoredFileName = Required(storedFileName, nameof(StoredFileName));
        UploadedAtUtc = Utc(uploadedAtUtc);
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
            throw new DomainException("Upload date must be UTC.");

        return value;
    }
}