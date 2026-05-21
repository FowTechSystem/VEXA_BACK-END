using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Tests.Bazar;

public class BazarTestFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    public AppDbContext Db { get; }
    public int EmpresaId { get; } = 1;
    public int UsuarioId { get; } = 1;

    public BazarTestFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        Db = new AppDbContext(options);
        Db.Database.EnsureCreated();
        Seed();
    }

    private void Seed()
    {
        Db.Empresas.Add(new Empresa("Teste", DateTime.UtcNow)
        {
            Id = EmpresaId,
            CNPJ = "00000000000000",
            Email = "t@test.com",
            Telefone = "11999999999"
        });
        Db.Usuarios.Add(new Usuario(EmpresaId, "Admin", "a@test.com", "admin", "hash", "Gerente")
        {
            Id = UsuarioId
        });
        Db.SaveChanges();
    }

    public BazarConsignante CriarConsignante(string nome = "Maria")
    {
        var c = new BazarConsignante
        {
            EmpresaId = EmpresaId,
            Nome = nome,
            Telefone = "(11) 99999-9999",
            Ativo = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };
        Db.BazarConsignantes.Add(c);
        Db.SaveChanges();
        return c;
    }

    public (BazarSacola sacola, BazarProduto peca) CriarSacolaComPeca(
        int consignanteId,
        BazarProdutoStatus status = BazarProdutoStatus.Disponivel)
    {
        var agora = DateTime.UtcNow;
        var sacola = new BazarSacola
        {
            EmpresaId = EmpresaId,
            ConsignanteId = consignanteId,
            DataCadastro = DateTime.UtcNow.Date,
            CriadoEm = agora,
            AtualizadoEm = agora
        };
        Db.BazarSacolas.Add(sacola);
        Db.SaveChanges();

        var peca = new BazarProduto
        {
            EmpresaId = EmpresaId,
            ConsignanteId = consignanteId,
            SacolaId = sacola.Id,
            Descricao = "Vestido teste",
            ValorCompra = 30,
            ValorVenda = 80,
            Status = status,
            DataEntrada = DateTime.UtcNow.Date,
            CriadoEm = agora,
            AtualizadoEm = agora
        };
        if (status == BazarProdutoStatus.Vendida)
            peca.DataVenda = DateTime.UtcNow.Date;
        Db.BazarProdutos.Add(peca);
        Db.SaveChanges();
        return (sacola, peca);
    }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}

public class FakeEmpresaContext : IEmpresaContext
{
    public FakeEmpresaContext(int empresaId) => EmpresaId = empresaId;
    public int EmpresaId { get; }
    public bool TemEmpresa => true;
}
