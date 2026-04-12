namespace CRMVEXA.Application.Features.Auth.ResetPassword;

public class ResetPasswordResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public static ResetPasswordResult Ok() => new() { Success = true };

    public static ResetPasswordResult Fail(string message) => new() { Success = false, ErrorMessage = message };
}
