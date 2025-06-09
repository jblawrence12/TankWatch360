using Microsoft.EntityFrameworkCore;
// ← add this:
using TankWatch.Domain;
public class TankContext : DbContext
{
    // DbContext for managing Tank and TankTelemetry entities
    // This context will be used by EF Core to interact with the database
    public TankContext(DbContextOptions<TankContext> opts) : base(opts) { }

    public DbSet<TankTelemetry> Telemetry => Set<TankTelemetry>();
    public DbSet<Tank>          Tanks     => Set<Tank>();    // ← new
}
