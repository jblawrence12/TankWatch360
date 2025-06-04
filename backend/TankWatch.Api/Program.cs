var builder = WebApplication.CreateBuilder(args);

// 1️⃣ add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2️⃣ enable Swagger UI in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();          // /swagger/v1/swagger.json
    app.UseSwaggerUI();        // /swagger/index.html
}

app.MapGet("/", () => "Hello TankWatch!");

app.Run();
