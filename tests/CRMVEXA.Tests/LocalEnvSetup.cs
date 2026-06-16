using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CRMVEXA.Tests;

/// <summary>
/// Seeder de ambiente local (NÃO é teste de regressão).
/// Roda apenas quando a variável VEXA_LOCAL_SETUP=1 está definida:
///   dotnet test --filter LocalEnvSetup
/// Cria o schema no Postgres local e semeia empresa, usuário admin e módulos.
/// </summary>
public class LocalEnvSetup
{
    [Fact]
    public async Task Provisionar_banco_local()
    {
        if (Environment.GetEnvironmentVariable("VEXA_LOCAL_SETUP") != "1")
            return;

        const string cs = "Host=localhost;Port=5432;Database=CRMVEXA;Username=postgres;Password=1234";
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(cs).Options;
        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        if (!await db.Empresas.AnyAsync())
        {
            db.Empresas.Add(new Empresa("Loja Teste", DateTime.UtcNow)
            {
                CNPJ = "00000000000191",
                Email = "loja@test.com",
                Telefone = "11999999999",
                Ativo = true
            });
            await db.SaveChangesAsync();
        }

        var empresa = await db.Empresas.FirstAsync();

        if (!await db.Usuarios.AnyAsync(u => u.Email == "admin@test.com"))
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("123456");
            db.Usuarios.Add(new Usuario(empresa.Id, "Admin", "admin@test.com", "admin", hash, "Gerente"));
            await db.SaveChangesAsync();
        }

        // Usuário dev (admin de desenvolvimento) para testar o painel de Personalização/Módulos.
        if (!await db.Usuarios.AnyAsync(u => u.Email == "dev@gmail.com"))
        {
            var hashDev = BCrypt.Net.BCrypt.HashPassword("dev12345");
            db.Usuarios.Add(new Usuario(empresa.Id, "Dev", "dev@gmail.com", "dev", hashDev, "Gerente"));
            await db.SaveChangesAsync();
        }

        string[] modulos = { "produtos", "clientes", "vendas", "caixa", "bazar", "repasses", "relatorios", "estoque" };
        foreach (var key in modulos)
        {
            if (!await db.EmpresaModulos.AnyAsync(m => m.EmpresaId == empresa.Id && m.ModuleKey == key))
            {
                db.EmpresaModulos.Add(new EmpresaModulo
                {
                    EmpresaId = empresa.Id,
                    ModuleKey = key,
                    IsEnabled = true,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
        await db.SaveChangesAsync();
    }
}
