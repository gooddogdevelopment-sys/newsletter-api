namespace dotnet_core_api_w_postgres.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
{
    private const string _apikeyname = "X-ApiKey";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(_apikeyname, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key missing.");

            logger.LogError("API Key missing in request.");
            return;
        }

        var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = appSettings.GetValue<string>("NewsletterSettings:ApiKey");

        if (apiKey is null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client.");

            logger.LogError("API Key is not configured.");
            return;
        }

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client.");

            logger.LogError("API Key does not match. {extractApiKey}", extractedApiKey.ToString().TakeLast(5));
            return;
        }

        await next(context);
    }
}
