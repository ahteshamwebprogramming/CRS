namespace CRS.Infrastruture.Helper;

public class ClientIpMiddleware
{
    private readonly RequestDelegate _next;

    public ClientIpMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        // You can log the IP or do other things with it here
        // For example:
        context.Items["ClientIp"] = clientIp;

        await _next(context);
    }
}
