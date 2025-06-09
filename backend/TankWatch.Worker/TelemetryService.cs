using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TankWatch.Worker
{
    /// <summary>
    /// TelemetryService is responsible for sending telemetry data to the API.
    /// It uses HttpClient to post telemetry data to the specified endpoint.
    /// </summary>
    public class TelemetryService
    {
        private readonly HttpClient _http; // HttpClient instance for making HTTP requests

        
        public TelemetryService(HttpClient http)
        {
            _http = http;
        }

        // Sends telemetry data to the API endpoint for broadcasting.
        public async Task SendTelemetryAsync(object telemetryDto, CancellationToken token)
        {
            var response = await _http.PostAsJsonAsync("http://localhost:5048/api/telemetry/broadcast", telemetryDto, token);
            response.EnsureSuccessStatusCode(); // Ensure the request was successful
        }
    }
}
