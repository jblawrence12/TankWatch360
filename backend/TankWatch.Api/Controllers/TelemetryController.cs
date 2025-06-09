using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankWatch.Domain;

namespace TankWatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly TankContext _db;
    public TelemetryController(TankContext db) => _db = db;

    [HttpGet("latest")]
    public async Task<IEnumerable<object>> GetLatest()
    {
        // 1) Include the Tank navigation so EF will JOIN on Tanks
        var raw = await _db.Telemetry
                           .Include(t => t.Tank)                     // <— Eager‐load the Tank
                           .OrderByDescending(t => t.Timestamp)
                           .Take(100)
                           .ToListAsync();

        // 2) Project into an anonymous object (or a DTO class) that includes tankName
        return raw.Select(t => new
        {
            t.Id,
            t.Timestamp,
            t.Level,
            t.Temperature,
            t.Radiation,
            t.Pressure,
            TankId = t.TankId,
            TankName = t.Tank?.Name                      // <— uses the included navigation
        });
    }
    
}
