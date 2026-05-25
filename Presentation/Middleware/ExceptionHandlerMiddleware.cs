using System.Net;
using System.Text.Json;
using CRMVEXA.Application.Exceptions;
using Npgsql;

namespace CRMVEXA.Presentation.Middleware;

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
        HttpStatusCode statusCode;
        object payload;

        if (ex is BazarException bazarEx)
        {
            statusCode = (HttpStatusCode)bazarEx.StatusCode;
            if (!string.IsNullOrEmpty(bazarEx.ErrorCode))
            {
                var dict = new Dictionary<string, object?>
                {
                    ["sucesso"] = false,
                    ["error"] = bazarEx.ErrorCode,
                    ["message"] = bazarEx.Message,
                    ["mensagem"] = bazarEx.Message
                };
                if (bazarEx.ExtraData != null)
                {
                    foreach (var kv in bazarEx.ExtraData)
                        dict[kv.Key] = kv.Value;
                }
                if (bazarEx.Detail != null) dict["detail"] = bazarEx.Detail;
                if (bazarEx.Errors != null) dict["errors"] = bazarEx.Errors;
                payload = dict;
            }
            else
            {
                payload = new
                {
                    sucesso = false,
                    mensagem = bazarEx.Message,
                    message = bazarEx.Message,
                    detail = bazarEx.Detail,
                    errors = bazarEx.Errors
                };
            }
        }
        else
        {
            statusCode = ex is InvalidOperationException or ArgumentException or ArgumentNullException or PostgresException
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            payload = new
            {
                sucesso = false,
                mensagem,
                message = mensagem
            };
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

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
