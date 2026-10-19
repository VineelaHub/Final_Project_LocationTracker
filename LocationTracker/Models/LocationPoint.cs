using SQLite;

namespace LocationTracker.Models;

/// <summary>Single GPS observation persisted on this device.</summary>
public sealed class LocationPoint
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? AccuracyMeters { get; set; }
    public DateTime TimestampUtc { get; set; }
}
