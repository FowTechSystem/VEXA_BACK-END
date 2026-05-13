namespace CRMVEXA.Application.Abstractions;

public interface IConnectionStringProtector
{
    string Protect(string plainText);
    string Unprotect(string protectedPayload);
}
