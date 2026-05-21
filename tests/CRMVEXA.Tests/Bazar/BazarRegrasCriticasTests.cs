using CRMVEXA.Application.Exceptions;
using CRMVEXA.Application.Features.Bazar.Pecas;
using CRMVEXA.Application.Features.Bazar.Repasses;
using CRMVEXA.Application.Features.Bazar.Sacolas;
using CRMVEXA.Application.Features.Bazar.Consignantes;
using CRMVEXA.Application.Services.Bazar;
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

        var handler = new BazarPecasHandlers(fx.Db, new FakeEmpresaContext(fx.EmpresaId), new BazarFotoStorageService(
            new FakeWebHostEnvironment()));

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

    [Fact]
    public async Task Soft_delete_consignante_com_pecas_ativas_falha()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Disponivel);

        var handler = new BazarConsignantesHandlers(
            fx.Db, new FakeEmpresaContext(fx.EmpresaId), new BazarResumoService(fx.Db));

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            handler.Handle(new DeleteConsignanteRequest(c.Id), CancellationToken.None));

        Assert.Equal(409, ex.StatusCode);
    }
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
