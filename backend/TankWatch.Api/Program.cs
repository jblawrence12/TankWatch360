using TankWatch.Api;
using TankWatch.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

/* ─── Services & Configuration ──────────────────────────────────────────────
 * Add services to the container.
 * This is where we configure EF Core, SignalR, CORS, etc.
 */
builder.Services.AddControllers();  // ← Add MVC controllers
builder.Services.AddEndpointsApiExplorer(); // ← For OpenAPI/Swagger support
builder.Services.AddSwaggerGen();       // ← Swagger UI for API docs

// ✅ Entity Framework Core
// This adds the EF Core services to the DI container
builder.Services.AddDbContext<TankContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ✅ SignalR
// This adds the SignalR services to the DI container
builder.Services.AddSignalR();

// ✅ CORS - Cross-Origin Resource Sharing
// This allows your Angular app to connect to the SignalR hub
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Your Angular dev URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ─── Build & Middleware ──────────────────────────────────────────────────────
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Middleware order matters!
app.UseRouting();               // ← This is required for MapHub
app.UseCors("AllowAll");        // ← Apply CORS to routes
app.UseAuthorization();         // ← Optional: If you're using auth
app.MapControllers();
app.MapHub<TelemetryHub>("/telemetryhub"); // ← Must match frontend SignalR URL

app.MapGet("/", () => "Hello TankWatch!");

// ─── Database Seeding ──────────────────────────────────────────────────────
// This is where we seed the database with initial data
// It runs once when the app starts, if the database is empty.
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<TankContext>();

    if (!ctx.Tanks.Any())
    {
        ctx.Tanks.AddRange(
            new Tank { Name = "Tank Alpha" },
            new Tank { Name = "Tank Bravo" },
            new Tank { Name = "Tank Charlie" }
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
                Id = rnd.Next(1000, 9999), // Random ID for demo purposes
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
