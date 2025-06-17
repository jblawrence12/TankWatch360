using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using TankWatch.Api;
using TankWatch.Domain;

var builder = WebApplication.CreateBuilder(args);

// ─── Services & Configuration ──────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Entity Framework Core
builder.Services.AddDbContext<TankContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
    b => b.MigrationsAssembly(typeof(Program).Assembly.FullName))
);

// ✅ SignalR
builder.Services.AddSignalR();

// ✅ CORS - Angular dev
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();
app.MapHub<TelemetryHub>("/telemetryhub");
app.MapGet("/", () => "Hello TankWatch!");

// ─── Database Seeding ──────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<TankContext>();

    // ✅ Ensure DB is created
    ctx.Database.EnsureCreated();

    if (!ctx.Tanks.Any())
    {
        ctx.Tanks.AddRange(
            new Tank
            {
                Name = "Tank Alpha",
                Latitude = 46.5501,
                Longitude = -119.5501
            },
            new Tank
            {
                Name = "Tank Bravo",
                Latitude = 46.5800,
                Longitude = -119.5906
            },
            new Tank
            {
                Name = "Tank Charlie",
                Latitude = 46.6000,
                Longitude = -119.6210
            }
        );
        ctx.SaveChanges();
    }

    var tanks = ctx.Tanks.ToArray();

    if (!ctx.Telemetry.Any())
    {
        var now = DateTime.UtcNow;
        var rnd = Random.Shared;

        var rows = Enumerable.Range(0, 200).Select(i =>
        {
            var chosenTank = tanks[rnd.Next(tanks.Length)];
            return new TankTelemetry
            {
                Id = rnd.Next(1000, 9999),
                Timestamp = now.AddMinutes(-i),
                Level = rnd.NextDouble() * 100,
                Temperature = 20 + rnd.NextDouble() * 50,
                Radiation = rnd.NextDouble() * 5,
                Pressure = 10 + rnd.NextDouble() * 10,
                TankId = chosenTank.Id,
                Tank = chosenTank
            };
        });

        ctx.Telemetry.AddRange(rows);
        ctx.SaveChanges();
    }
}

app.Run();
