namespace CRMVEXA.Application.Exceptions;

public class BazarException : Exception
{
    public int StatusCode { get; }
    public string? Detail { get; }
    public Dictionary<string, string[]>? Errors { get; }

    public BazarException(string message, int statusCode = 400, string? detail = null, Dictionary<string, string[]>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Detail = detail;
        Errors = errors;
    }

    public static BazarException NotFound(string message = "Registro não encontrado.") =>
        new(message, 404);

    public static BazarException Conflict(string message) =>
        new(message, 409);

    public static BazarException BusinessRule(string message) =>
        new(message, 422);

    public static BazarException Validation(string message, Dictionary<string, string[]>? errors = null) =>
        new(message, 400, errors: errors);
}
