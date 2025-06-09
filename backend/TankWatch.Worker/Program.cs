using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using TankWatch.Api;          // ✅ For TelemetryHub
using TankWatch.Domain;       // ✅ For TankContext
using TankWatch.Worker;       // ✅ For Worker

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add your DB context
        services.AddDbContext<TankContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("Default")));

        // Add SignalR (required for IHubContext<T>)
        services.AddSignalR();


        // Register the background worker
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
