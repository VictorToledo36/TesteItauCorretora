namespace TesteItauCorretora.Middleware;

public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Gera um UUID único por requisição
        var requestId = Guid.NewGuid().ToString();

        // Adiciona no response header antes de processar
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Request-Id"] = requestId;
            context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
            return Task.CompletedTask;
        });

        await _next(context);
    }
}