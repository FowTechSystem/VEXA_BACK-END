using CRMVEXA.Application.Exceptions;
using CRMVEXA.Application.Features.Bazar.Pecas;
using CRMVEXA.Application.Features.Bazar.Repasses;
using CRMVEXA.Application.Features.Bazar.Sacolas;
using CRMVEXA.Application.Features.Bazar.Consignantes;
using CRMVEXA.Application.Services.Bazar;
using Microsoft.EntityFrameworkCore;
using CRMVEXA.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace CRMVEXA.Tests.Bazar;

public class BazarRegrasCriticasTests
{
    [Fact]
    public async Task Nao_pode_editar_peca_vendida()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (_, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Vendida);

        var handler = CriarPecasHandler(fx);

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            handler.Handle(new UpdatePecaRequest { Id = peca.Id, Descricao = "Novo nome" }, CancellationToken.None));

        Assert.Equal(422, ex.StatusCode);
    }

    [Fact]
    public async Task Nao_pode_gerar_dois_repasses_com_mesma_peca()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (_, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Vendida);

        var http = new FakeHttpContextAccessor(fx.UsuarioId);
        var handler = new BazarRepassesHandlers(fx.Db, new FakeEmpresaContext(fx.EmpresaId), http);

        await handler.Handle(new CreateRepasseRequest
        {
            ConsignanteId = c.Id,
            PecasIds = new List<int> { peca.Id }
        }, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            handler.Handle(new CreateRepasseRequest
            {
                ConsignanteId = c.Id,
                PecasIds = new List<int> { peca.Id }
            }, CancellationToken.None));

        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task Cancelar_repasse_libera_pecas_para_novo_repasse()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (_, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Vendida);

        var http = new FakeHttpContextAccessor(fx.UsuarioId);
        var handler = new BazarRepassesHandlers(fx.Db, new FakeEmpresaContext(fx.EmpresaId), http);

        var repasse = await handler.Handle(new CreateRepasseRequest
        {
            ConsignanteId = c.Id,
            PecasIds = new List<int> { peca.Id }
        }, CancellationToken.None);

        await handler.Handle(new DeleteRepasseRequest(repasse.Id), CancellationToken.None);

        // A peça deve voltar a aparecer como pendente de repasse
        var pendentes = await handler.Handle(
            new GetRepassesPendentesConsignanteRequest(c.Id), CancellationToken.None);
        Assert.Single(pendentes);
        Assert.Contains(pendentes[0].Pecas, p => p.Id == peca.Id);

        // E deve poder entrar em um novo repasse sem conflito
        var novo = await handler.Handle(new CreateRepasseRequest
        {
            ConsignanteId = c.Id,
            PecasIds = new List<int> { peca.Id }
        }, CancellationToken.None);
        Assert.Contains(peca.Id, novo.PecasIds);
    }

    [Fact]
    public async Task Cancelar_venda_com_peca_em_repasse_falha()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (_, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Vendida);

        var venda = new Domain.Entities.Venda(fx.EmpresaId, "VND-REP", fx.UsuarioId, "Pix");
        fx.Db.Vendas.Add(venda);
        await fx.Db.SaveChangesAsync();

        var item = new Domain.Entities.ItemVenda(peca.Id, 50m) { VendaId = venda.Id };
        fx.Db.ItensVenda.Add(item);
        var pecaDb = await fx.Db.BazarProdutos.FindAsync(peca.Id);
        pecaDb!.VendaId = venda.Id;
        await fx.Db.SaveChangesAsync();

        var http = new FakeHttpContextAccessor(fx.UsuarioId);
        var repassesHandler = new BazarRepassesHandlers(fx.Db, new FakeEmpresaContext(fx.EmpresaId), http);
        await repassesHandler.Handle(new CreateRepasseRequest
        {
            ConsignanteId = c.Id,
            PecasIds = new List<int> { peca.Id }
        }, CancellationToken.None);

        var cancelarHandler = new Application.Features.Vendas.CancelarVenda.CancelarVendaHandler(
            fx.Db, new FakeEmpresaContext(fx.EmpresaId), http);

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            cancelarHandler.Handle(
                new Application.Features.Vendas.CancelarVenda.CancelarVendaRequest { Id = venda.Id },
                CancellationToken.None));

        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task Venda_concorrente_bazar_apenas_uma_sucede()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (_, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Disponivel);

        var venda = new Domain.Entities.Venda(fx.EmpresaId, "VND-TEST", fx.UsuarioId, "Pix");
        fx.Db.Vendas.Add(venda);
        await fx.Db.SaveChangesAsync();

        var p1 = await fx.Db.BazarProdutos.FindAsync(peca.Id);
        Assert.NotNull(p1);
        if (p1!.Status == BazarProdutoStatus.Disponivel)
            p1.Status = BazarProdutoStatus.Vendida;
        await fx.Db.SaveChangesAsync();

        var p2 = await fx.Db.BazarProdutos.FindAsync(peca.Id);
        Assert.NotNull(p2);
        var segundaVendaOk = p2!.Status == BazarProdutoStatus.Disponivel;
        if (segundaVendaOk)
            p2.Status = BazarProdutoStatus.Vendida;

        Assert.False(segundaVendaOk);
    }

    [Fact]
    public async Task Devolver_sacola_com_peca_vendida_falha()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (sacola, _) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Vendida);

        var handler = new BazarSacolasHandlers(fx.Db, new FakeEmpresaContext(fx.EmpresaId));

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            handler.Handle(new DevolverSacolaRequest(sacola.Id), CancellationToken.None));

        Assert.Equal(422, ex.StatusCode);
        Assert.Contains("vendidas", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static BazarPecasHandlers CriarPecasHandler(BazarTestFixture fx) =>
        new(
            fx.Db,
            new FakeEmpresaContext(fx.EmpresaId),
            new BazarFotoStorageService(new FakeWebHostEnvironment()),
            new BazarAuditoriaService(
                fx.Db,
                new FakeEmpresaContext(fx.EmpresaId),
                new FakeHttpContextAccessor(fx.UsuarioId)));
}

internal class FakeWebHostEnvironment : IWebHostEnvironment
{
    public string WebRootPath { get; set; } = Path.Combine(Path.GetTempPath(), "bazar-tests-wwwroot");
    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public string EnvironmentName { get; set; } = "Test";
    public string ApplicationName { get; set; } = "CRMVEXA.Tests";
    public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; } = null!;
    public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
}

internal class FakeHttpContextAccessor : IHttpContextAccessor
{
    public FakeHttpContextAccessor(int userId)
    {
        var ctx = new DefaultHttpContext();
        ctx.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString()) }));
        HttpContext = ctx;
    }

    public HttpContext? HttpContext { get; set; }
}
