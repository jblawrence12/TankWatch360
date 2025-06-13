using TankWatch.Domain;

public class Tank
{
    public int    Id        { get; set; }
    public string Name      { get; set; } = default!;

    // Geographic position of the physical tank
    public double Latitude  { get; set; }
    public double Longitude { get; set; }

    public ICollection<TankTelemetry> Telemetry { get; set; } = new List<TankTelemetry>();
}
