using System.Net;
using System.Text.Json;
using Npgsql;

namespace CRMVEXA.API.Middleware;

/// <summary>
/// Captura exceções não tratadas e retorna resposta JSON padronizada (sucesso, mensagem, message).
/// Evita respostas 500 sem corpo útil para o frontend.
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção não tratada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var mensagem = ObterMensagem(ex);
        var statusCode = ex is InvalidOperationException or ArgumentException or ArgumentNullException or PostgresException
            ? HttpStatusCode.BadRequest
            : HttpStatusCode.InternalServerError;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new
        {
            sucesso = false,
            mensagem,
            message = mensagem
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private static string ObterMensagem(Exception ex)
    {
        var atual = ex;
        while (atual?.InnerException != null)
            atual = atual.InnerException;
        return atual?.Message ?? ex.Message;
    }
}
