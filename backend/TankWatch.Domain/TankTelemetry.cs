namespace TankWatch.Domain;

public class TankTelemetry
{
    public int      Id          { get; set; }
    public DateTime Timestamp   { get; set; }
    public double   Level       { get; set; }
    public double   Temperature { get; set; }
    public double   Radiation   { get; set; }

    // ← New properties:
    public int   TankId { get; set; }
    
    public double Pressure { get; set; } // Added pressure for variety
    public Tank? Tank { get; set; }
}
