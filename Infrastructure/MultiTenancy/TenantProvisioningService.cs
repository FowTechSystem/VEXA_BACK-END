using CRMVEXA.Application.Abstractions;
using CRMVEXA.Application.Services.Personalizacao;
using CRMVEXA.Domain.Entities.Master;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CRMVEXA.Infrastructure.MultiTenancy;

public sealed class TenantProvisioningService : ITenantProvisioningService
{
    private readonly MasterDbContext _master;
    private readonly IConfiguration _configuration;

    public TenantProvisioningService(MasterDbContext master, IConfiguration configuration)
    {
        _master = master;
        _configuration = configuration;
    }

    public async Task<int> ProvisionAsync(
        string nome,
        string? nomeFantasia,
        string? cnpj,
        string? email,
        string? telefone,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        var adminCs = _configuration.GetConnectionString("MasterAdminConnection")
            ?? _configuration.GetConnectionString("MasterConnection")
            ?? throw new InvalidOperationException("Configure MasterAdminConnection ou MasterConnection para provisionar bancos.");

        var dataCadastro = DateTime.UtcNow;
        var tenant = new TenantCatalog
        {
            Nome = nome.Trim(),
            NomeFantasia = string.IsNullOrWhiteSpace(nomeFantasia) ? null : nomeFantasia.Trim(),
            Cnpj = string.IsNullOrWhiteSpace(cnpj) ? null : cnpj.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim(),
            Ativo = ativo,
            DataCadastro = dataCadastro,
            DatabaseName = ""
        };

        _master.Tenants.Add(tenant);
        await _master.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var tenantId = tenant.Id;
        var dbName = $"crmvexa_tenant_{tenantId}";

        await using (var conn = new NpgsqlConnection(adminCs))
        {
            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
            var quoted = dbName.Replace("\"", "\"\"");
            await using var cmd = new NpgsqlCommand($"CREATE DATABASE \"{quoted}\" WITH TEMPLATE = template0 ENCODING 'UTF8';", conn);
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        tenant.DatabaseName = dbName;
        tenant.ProvisionadoEm = DateTime.UtcNow;
        await _master.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var tenantCs = BuildTenantConnectionString(dbName);
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(tenantCs)
            .Options;

        await using (var tenantCtx = new AppDbContext(options))
        {
            await tenantCtx.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        await using (var seedCtx = new AppDbContext(options))
        {
            await seedCtx.Database.ExecuteSqlInterpolatedAsync(
                $@"
                INSERT INTO ""Empresas"" (""Id"", ""Nome"", ""NomeFantasia"", ""CNPJ"", ""Email"", ""Telefone"", ""Ativo"", ""DataCadastro"")
                VALUES ({tenantId}, {tenant.Nome}, {tenant.NomeFantasia}, {tenant.Cnpj}, {tenant.Email}, {tenant.Telefone}, {tenant.Ativo}, {dataCadastro})",
                cancellationToken).ConfigureAwait(false);

            await seedCtx.Database.ExecuteSqlRawAsync(
                """
                SELECT setval(pg_get_serial_sequence('"Empresas"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Empresas"), 1));
                """,
                cancellationToken).ConfigureAwait(false);

            await EmpresaPersonalizacaoSeed.GarantirPadraoAsync(seedCtx, tenantId, tenant.Nome, ct: cancellationToken)
                .ConfigureAwait(false);
        }

        return tenantId;
    }

    private string BuildTenantConnectionString(string databaseName)
    {
        var template = _configuration["ConnectionStrings:TenantConnectionTemplate"]
            ?? throw new InvalidOperationException("ConnectionStrings:TenantConnectionTemplate não configurado.");
        return template.Replace("{Database}", databaseName, StringComparison.OrdinalIgnoreCase);
    }
}
