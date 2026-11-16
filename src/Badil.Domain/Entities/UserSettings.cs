using Badil.Domain.Common;

namespace Badil.Domain.Entities;

public sealed class UserSettings
{
    // Shared primary key with Identity user
    public Guid UserId { get; private set; }

    public bool EmailAlerts { get; private set; }

    public bool MatchAlerts { get; private set; }

    public bool MessageAlerts { get; private set; }

    public bool DealAlerts { get; private set; }

    private UserSettings()
    {
    }

    public UserSettings(
        Guid userId,
        bool emailAlerts,
        bool matchAlerts,
        bool messageAlerts,
        bool dealAlerts)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID is required.");

        UserId = userId;
        EmailAlerts = emailAlerts;
        MatchAlerts = matchAlerts;
        MessageAlerts = messageAlerts;
        DealAlerts = dealAlerts;
    }

    public void Update(
        bool email,
        bool match,
        bool message,
        bool deal)
    {
        EmailAlerts = email;
        MatchAlerts = match;
        MessageAlerts = message;
        DealAlerts = deal;
    }
}