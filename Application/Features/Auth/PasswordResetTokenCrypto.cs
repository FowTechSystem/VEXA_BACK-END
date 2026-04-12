using System.Security.Cryptography;
using System.Text;

namespace CRMVEXA.Application.Features.Auth;

public static class PasswordResetTokenCrypto
{
    /// <summary>Gera token URL-safe e o hash (SHA-256) para persistir em Base64.</summary>
    public static (string PlainToken, string HashBase64) CreateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var plain = Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(plain));
        return (plain, Convert.ToBase64String(hash));
    }

    public static string HashPlainToken(string plainToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(plainToken));
        return Convert.ToBase64String(hash);
    }
}
