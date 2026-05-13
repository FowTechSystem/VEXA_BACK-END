using CRMVEXA.Application.Abstractions;
using Microsoft.AspNetCore.DataProtection;

namespace CRMVEXA.Infrastructure.Security;

public sealed class DataProtectionConnectionStringProtector : IConnectionStringProtector
{
    private readonly IDataProtector _protector;

    public DataProtectionConnectionStringProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("CRMVEXA.Tenant.ConnectionStrings.v1");
    }

    public string Protect(string plainText) => _protector.Protect(plainText);

    public string Unprotect(string protectedPayload) => _protector.Unprotect(protectedPayload);
}
