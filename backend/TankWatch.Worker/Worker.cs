using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TankWatch.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace TankWatch.Worker
{
    /// <summary>
    /// Worker is a background service that periodically generates and sends telemetry data for tanks.
    /// It retrieves tanks from the database, generates random telemetry data, and sends it via SignalR.
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TelemetryService _telemetry;

        public Worker(IServiceProvider services, TelemetryService telemetry)
        {
            _services = services;
            _telemetry = telemetry;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var ctx = scope.ServiceProvider.GetRequiredService<TankContext>();

                    // ✅ Use AsNoTracking to load full tank data including lat/lng
                    var tanks = await ctx.Tanks
                                         .AsNoTracking()
                                         .ToListAsync(stoppingToken);

                    if (tanks.Any())
                    {
                        var rnd = Random.Shared;
                        var chosenTank = tanks[rnd.Next(tanks.Count)];

                        var newTelemetry = new TankTelemetry
                        {
                            Id = 0, // DB-generated
                            Timestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local),
                            Level = rnd.NextDouble() * 100,
                            Temperature = 20 + rnd.NextDouble() * 50,
                            Radiation = rnd.NextDouble() * 5,
                            Pressure = 10 + rnd.NextDouble() * 10,
                            TankId = chosenTank.Id
                        };

                        ctx.Telemetry.Add(newTelemetry);
                        await ctx.SaveChangesAsync(stoppingToken);

                        var telemetryToSend = new
                        {
                            Id = newTelemetry.Id,
                            Timestamp = newTelemetry.Timestamp,
                            Level = newTelemetry.Level,
                            Temperature = newTelemetry.Temperature,
                            Radiation = newTelemetry.Radiation,
                            Pressure = newTelemetry.Pressure,
                            TankId = newTelemetry.TankId,
                            TankName = chosenTank.Name,
                            Lat = chosenTank.Latitude,
                            Lng = chosenTank.Longitude
                        };

                        Console.WriteLine($"📡 Preparing telemetry: {JsonSerializer.Serialize(telemetryToSend)}");

                        await _telemetry.SendTelemetryAsync(telemetryToSend, stoppingToken);

                        Console.WriteLine($"✅ Sent telemetry: {JsonSerializer.Serialize(telemetryToSend)}");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ No tanks found in the database.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error in telemetry worker: {ex.Message}\n{ex.StackTrace}");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
