using Microsoft.AspNetCore.SignalR;

namespace TankWatch.Api// Adjust namespace as needed
{
    /// TelemetryHub is a SignalR hub for broadcasting telemetry data
    /// to connected clients. Clients can subscribe to receive real-time updates
    public class TelemetryHub : Hub
    {
        // Optional: You could add groups or auth logic here later
    }
}
