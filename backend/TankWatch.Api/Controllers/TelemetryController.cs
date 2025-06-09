using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TankWatch.Domain;
using TankWatch.Api;
using Microsoft.EntityFrameworkCore;

//Summary:
// This controller handles telemetry data for tanks, providing endpoints to retrieve the latest telemetry records
// and broadcast telemetry updates to connected clients via SignalR.
//Summary
[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly TankContext _db; // Database context for accessing telemetry data
    private readonly IHubContext<TelemetryHub> _hub;           // SignalR hub context

    public TelemetryController(TankContext db, IHubContext<TelemetryHub> hub)
    {
        _db = db;
        _hub = hub;
    }


    // GET api/telemetry/latest
    // Returns the latest 100 telemetry records, including tank names
    [HttpGet("latest")]
    public async Task<IEnumerable<object>> GetLatest()
    {
        var raw = await _db.Telemetry
                           .Include(t => t.Tank)
                           .OrderByDescending(t => t.Timestamp)
                           .Take(100)
                           .ToListAsync();
        // Project into an anonymous DTO that contains TankName
            //     (exactly what the Angular Telemetry interface expects).
        return raw.Select(t => new
        {
            t.Id,
            t.Timestamp,
            t.Level,
            t.Temperature,
            t.Radiation,
            t.Pressure,
            TankId = t.TankId,
            TankName = t.Tank?.Name
        });
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> BroadcastTelemetry([FromBody] object telemetryDto)
    {
        // Push to every connected SignalR client
            // Client-side handler is registered as hubConnection.on("ReceiveTelemetry")
        await _hub.Clients.All.SendAsync("ReceiveTelemetry", telemetryDto);
        return Ok();
    }
}
