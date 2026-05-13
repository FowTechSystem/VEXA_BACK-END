using CRMVEXA.Application.Abstractions;
using CRMVEXA.Application.Features.Auth;
using CRMVEXA.Application.Features.Auth.ResetPassword;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.MultiTenancy;

public interface ICrossTenantResolver
{
    Task<UsuarioAuthSnapshot?> ResolveUsuarioLoginAsync(string email, CancellationToken cancellationToken = default);

    Task<UsuarioAuthSnapshot?> ResolveUsuarioForPasswordResetAsync(string email, CancellationToken cancellationToken = default);

    Task<CatalogoPublicSnapshot?> FindCatalogoBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<CatalogoPublicSnapshot?> FindCatalogoByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ResetPasswordResult> TryResetPasswordAsync(string plainToken, string novaSenha, CancellationToken cancellationToken = default);
}

public sealed class UsuarioAuthSnapshot
{
    public required Usuario Usuario { get; init; }
    public required int TenantId { get; init; }
}

public sealed class CatalogoPublicSnapshot
{
    public required Catalogo Catalogo { get; init; }
    public required int TenantId { get; init; }
}

public sealed class CrossTenantResolver : ICrossTenantResolver
{
    private readonly MasterDbContext _master;
    private readonly ITenantAppDbContextFactory _dbFactory;
    private readonly IUserTenantDirectoryService _directory;

    public CrossTenantResolver(
        MasterDbContext master,
        ITenantAppDbContextFactory dbFactory,
        IUserTenantDirectoryService directory)
    {
        _master = master;
        _dbFactory = dbFactory;
        _directory = directory;
    }

    public async Task<UsuarioAuthSnapshot?> ResolveUsuarioLoginAsync(string email, CancellationToken cancellationToken = default)
    {
        var norm = NormalizeEmail(email);
        if (string.IsNullOrEmpty(norm))
            return null;

        var tenantFromDirectory = await _directory.GetTenantIdByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        if (tenantFromDirectory is int tidFromDir)
        {
            await using var ctx = _dbFactory.CreateDbContext(tidFromDir);
            var u = await ctx.Usuarios
                .AsNoTracking()
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(x => x.Email.ToLower() == norm, cancellationToken)
                .ConfigureAwait(false);
            if (u != null)
                return new UsuarioAuthSnapshot { Usuario = u, TenantId = tidFromDir };
        }

        var tenantIds = await _master.Tenants.AsNoTracking()
            .Where(t => t.Ativo)
            .OrderBy(t => t.Id)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var tid in tenantIds)
        {
            await using var ctx = _dbFactory.CreateDbContext(tid);
            var u = await ctx.Usuarios
                .AsNoTracking()
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(x => x.Email.ToLower() == norm, cancellationToken)
                .ConfigureAwait(false);
            if (u == null)
                continue;

            await _directory.UpsertAsync(tid, norm, u.Id, cancellationToken).ConfigureAwait(false);
            return new UsuarioAuthSnapshot { Usuario = u, TenantId = tid };
        }

        return null;
    }

    public Task<UsuarioAuthSnapshot?> ResolveUsuarioForPasswordResetAsync(string email, CancellationToken cancellationToken = default) =>
        ResolveUsuarioLoginAsync(email, cancellationToken);

    public async Task<CatalogoPublicSnapshot?> FindCatalogoBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;
        var s = slug.Trim();

        var tenantIds = await _master.Tenants.AsNoTracking()
            .Where(t => t.Ativo)
            .OrderBy(t => t.Id)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var tid in tenantIds)
        {
            await using var ctx = _dbFactory.CreateDbContext(tid);
            var c = await ctx.Catalogos
                .AsNoTracking()
                .Include(x => x.Produtos)
                .FirstOrDefaultAsync(x => x.Slug == s && x.Ativo, cancellationToken)
                .ConfigureAwait(false);
            if (c != null)
                return new CatalogoPublicSnapshot { Catalogo = c, TenantId = tid };
        }

        return null;
    }

    public async Task<CatalogoPublicSnapshot?> FindCatalogoByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenantIds = await _master.Tenants.AsNoTracking()
            .Where(t => t.Ativo)
            .OrderBy(t => t.Id)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var tid in tenantIds)
        {
            await using var ctx = _dbFactory.CreateDbContext(tid);
            var c = await ctx.Catalogos
                .AsNoTracking()
                .Include(x => x.Produtos)
                .FirstOrDefaultAsync(x => x.Id == id && x.Ativo, cancellationToken)
                .ConfigureAwait(false);
            if (c != null)
                return new CatalogoPublicSnapshot { Catalogo = c, TenantId = tid };
        }

        return null;
    }

    public async Task<ResetPasswordResult> TryResetPasswordAsync(string plainToken, string novaSenha, CancellationToken cancellationToken = default)
    {
        var token = plainToken.Trim();
        if (string.IsNullOrEmpty(token))
            return ResetPasswordResult.Fail("Token inválido ou expirado.");

        var agora = DateTime.UtcNow;
        var hashB64 = PasswordResetTokenCrypto.HashPlainToken(token);

        var tenantIds = await _master.Tenants.AsNoTracking()
            .Where(t => t.Ativo)
            .OrderBy(t => t.Id)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var tid in tenantIds)
        {
            await using var ctx = _dbFactory.CreateDbContext(tid);
            var row = await ctx.PasswordResetTokens
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(
                    t => t.TokenHash == hashB64 && t.UsedAt == null && t.ExpiresAt > agora,
                    cancellationToken)
                .ConfigureAwait(false);

            if (row == null)
                continue;

            if (!row.Usuario.Ativo)
                return ResetPasswordResult.Fail("Token inválido ou expirado.");

            row.Usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            row.UsedAt = agora;
            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return ResetPasswordResult.Ok();
        }

        return ResetPasswordResult.Fail("Token inválido ou expirado.");
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
