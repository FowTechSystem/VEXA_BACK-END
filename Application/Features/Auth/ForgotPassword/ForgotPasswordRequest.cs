using MediatR;

namespace CRMVEXA.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordRequest : IRequest<ForgotPasswordResponse>
{
    public string Email { get; set; } = string.Empty;
}
