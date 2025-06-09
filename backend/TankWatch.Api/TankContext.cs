using Microsoft.EntityFrameworkCore;
// ← add this:
using TankWatch.Domain;
public class TankContext : DbContext
{
    public TankContext(DbContextOptions<TankContext> opts) : base(opts) { }

    public DbSet<TankTelemetry> Telemetry => Set<TankTelemetry>();
    public DbSet<Tank>          Tanks     => Set<Tank>();    // ← new
}
