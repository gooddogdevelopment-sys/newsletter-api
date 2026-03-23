using dotnet_core_api_w_postgres.Data;
using dotnet_core_api_w_postgres.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

// Database
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Exception handling (RFC 9457 Problem Details)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Template", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });
});
builder.Services.AddOpenApi();


var app = builder.Build();

// Show swagger docs in all environments. Uncomment below if you want to limit to development
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseSerilogRequestLogging(); //Optional. Logs all Http requests

app.UseAuthorization();

app.MapControllers();

app.Run();