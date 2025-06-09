using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TankWatch.Domain;
using TankWatch.Worker;

// Main entry point for the Worker service
// This service will run in the background and periodically send telemetry data
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<TankContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("Default")));// Configure EF Core with SQL Server connection string

        services.AddHttpClient<TelemetryService>(); // Add HttpClient for TelemetryService
        services.AddHostedService<Worker>(); // Register the Worker service
    })
    .Build();

host.Run();
