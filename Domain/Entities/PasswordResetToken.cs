namespace CRMVEXA.Domain.Entities;

public class PasswordResetToken
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    /// <summary>SHA-256 em Base64 do token opaco (nunca o token em claro).</summary>
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
