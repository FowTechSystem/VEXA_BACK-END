using CRMVEXA.Application.Abstractions;
using CRMVEXA.Application.Services.Personalizacao;
using CRMVEXA.Domain.Entities.Master;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
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
        var databaseCreated = false;

        try
        {
            await using (var conn = new NpgsqlConnection(adminCs))
            {
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
                var quoted = dbName.Replace("\"", "\"\"");
                await using var cmd = new NpgsqlCommand($"CREATE DATABASE \"{quoted}\" WITH TEMPLATE = template0 ENCODING 'UTF8';", conn);
                await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            databaseCreated = true;

            tenant.DatabaseName = dbName;
            tenant.ProvisionadoEm = DateTime.UtcNow;
            await _master.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var tenantCs = BuildTenantConnectionString(dbName);
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(tenantCs)
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
                .Options;

            await using (var tenantCtx = new AppDbContext(options))
            {
                // IMPORTANTE: o schema é criado a partir do MODELO completo, não via
                // Database.MigrateAsync(). O histórico de migrations do projeto é
                // inconsistente (tabelas base como "Empresas"/"MovimentacoesEstoque"
                // não são criadas na ordem certa por nenhuma migration), então
                // MigrateAsync produzia um banco quebrado/incompleto. CreateTables()
                // gera o schema correto e completo direto do modelo do AppDbContext.
                var creator = (RelationalDatabaseCreator)tenantCtx.GetService<IRelationalDatabaseCreator>();
                await creator.CreateTablesAsync(cancellationToken).ConfigureAwait(false);

                // Baseline: registra todas as migrations como já aplicadas, para que o
                // banco fique consistente caso futuras migrations sejam adicionadas e
                // aplicadas via MigrateAsync (só as novas serão executadas).
                var history = tenantCtx.GetService<IHistoryRepository>();
                await tenantCtx.Database.ExecuteSqlRawAsync(history.GetCreateIfNotExistsScript(), cancellationToken)
                    .ConfigureAwait(false);

                var efVersion = ProductInfo.GetVersion();
                foreach (var migrationId in tenantCtx.Database.GetMigrations())
                {
                    var insertScript = history.GetInsertScript(new HistoryRow(migrationId, efVersion));
                    await tenantCtx.Database.ExecuteSqlRawAsync(insertScript, cancellationToken).ConfigureAwait(false);
                }
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
        catch
        {
            // Provisionamento é tudo-ou-nada: se algo falhar no meio, desfaz o
            // registro no Master e descarta o banco recém-criado para não deixar
            // uma empresa "zumbi" na lista (aparece, mas sem schema = inutilizável).
            await TryRollbackAsync(tenant, dbName, databaseCreated, adminCs).ConfigureAwait(false);
            throw;
        }
    }

    private async Task TryRollbackAsync(TenantCatalog tenant, string dbName, bool databaseCreated, string adminCs)
    {
        try
        {
            _master.Tenants.Remove(tenant);
            await _master.SaveChangesAsync().ConfigureAwait(false);
        }
        catch
        {
            // ignora: best-effort para não mascarar a exceção original
        }

        if (!databaseCreated)
            return;

        try
        {
            await using var conn = new NpgsqlConnection(adminCs);
            await conn.OpenAsync().ConfigureAwait(false);
            var quoted = dbName.Replace("\"", "\"\"");
            await using var cmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{quoted}\" WITH (FORCE);", conn);
            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        catch
        {
            // ignora: best-effort
        }
    }

    private string BuildTenantConnectionString(string databaseName)
    {
        var template = _configuration["ConnectionStrings:TenantConnectionTemplate"]
            ?? throw new InvalidOperationException("ConnectionStrings:TenantConnectionTemplate não configurado.");
        return template.Replace("{Database}", databaseName, StringComparison.OrdinalIgnoreCase);
    }
}
