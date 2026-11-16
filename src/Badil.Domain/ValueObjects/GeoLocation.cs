using Badil.Domain.Common;

namespace Badil.Domain.ValueObjects;

public sealed record GeoLocation
{
    public double Latitude { get; }

    public double Longitude { get; }

    public GeoLocation(
        double latitude,
        double longitude)
    {
        if (!double.IsFinite(latitude))
        {
            throw new DomainException(
                "Latitude must be a finite number.");
        }

        if (!double.IsFinite(longitude))
        {
            throw new DomainException(
                "Longitude must be a finite number.");
        }

        if (latitude < -90 || latitude > 90)
        {
            throw new DomainException(
                "Latitude must be between -90 and 90.");
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new DomainException(
                "Longitude must be between -180 and 180.");
        }

        Latitude = latitude;
        Longitude = longitude;
    }
}