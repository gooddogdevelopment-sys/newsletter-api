using dotnet_core_api_w_postgres.Data;
using dotnet_core_api_w_postgres.Middleware;
using dotnet_core_api_w_postgres.Services;
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
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Exception handling (RFC 9457 Problem Details)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Template", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. X-ApiKey: Your_Key",
        In = ParameterLocation.Header,
        Name = "X-ApiKey",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
    });
});
builder.Services.AddOpenApi();

builder.Services.AddScoped<INewsLetterService, NewsLetterService>();

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

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();