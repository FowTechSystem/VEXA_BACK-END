using CRMVEXA.Application.Exceptions;
using CRMVEXA.Application.Features.Bazar.Consignantes;
using CRMVEXA.Application.Features.Bazar.Pecas;
using CRMVEXA.Application.Services.Bazar;
using CRMVEXA.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CRMVEXA.Tests.Bazar;

public class BazarDeleteRulesTests
{
    [Fact]
    public async Task Excluir_consignante_com_vendas_retorna_HAS_SALES()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Vendida);

        var handler = CriarConsignantesHandler(fx);

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            handler.Handle(new DeleteConsignanteRequest(c.Id), CancellationToken.None));

        Assert.Equal(409, ex.StatusCode);
        Assert.Equal("HAS_SALES", ex.ErrorCode);
        Assert.Equal(1, ex.ExtraData?["salesCount"]);
    }

    [Fact]
    public async Task Excluir_consignante_com_pecas_em_sacola_sem_confirmacao_retorna_HAS_BAG_ITEMS()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Disponivel);

        var handler = CriarConsignantesHandler(fx);

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            handler.Handle(new DeleteConsignanteRequest(c.Id), CancellationToken.None));

        Assert.Equal(409, ex.StatusCode);
        Assert.Equal("HAS_BAG_ITEMS", ex.ErrorCode);
        Assert.Equal(1, ex.ExtraData?["bagItemsCount"]);
        Assert.Equal(true, ex.ExtraData?["requiresConfirmation"]);
    }

    [Fact]
    public async Task Excluir_consignante_com_pecas_em_sacola_confirmado_exclui_em_cascata()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (sacola, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Disponivel);

        var handler = CriarConsignantesHandler(fx);
        await handler.Handle(new DeleteConsignanteRequest(c.Id, Confirmed: true), CancellationToken.None);

        var consignante = await fx.Db.BazarConsignantes.FindAsync(c.Id);
        Assert.NotNull(consignante);
        Assert.NotNull(consignante!.DeletadoEm);

        Assert.False(await fx.Db.BazarProdutos.AnyAsync(p => p.Id == peca.Id));
        Assert.False(await fx.Db.BazarSacolas.AnyAsync(s => s.Id == sacola.Id));

        var auditoria = await fx.Db.AuditoriaAcoes
            .Where(a => a.Acao == "bazar.excluir.consignante" && a.RecursoId == c.Id.ToString())
            .ToListAsync();
        Assert.Single(auditoria);
    }

    [Fact]
    public async Task Excluir_consignante_sem_vinculos_retorna_sucesso()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();

        var handler = CriarConsignantesHandler(fx);
        await handler.Handle(new DeleteConsignanteRequest(c.Id), CancellationToken.None);

        var consignante = await fx.Db.BazarConsignantes.FindAsync(c.Id);
        Assert.NotNull(consignante);
        Assert.NotNull(consignante!.DeletadoEm);
        Assert.False(consignante.Ativo);
    }

    [Fact]
    public async Task Excluir_peca_vendida_retorna_PRODUCT_HAS_SALE()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (_, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Vendida);

        var handler = CriarPecasHandler(fx);

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            handler.Handle(new DeletePecaRequest(peca.Id), CancellationToken.None));

        Assert.Equal(409, ex.StatusCode);
        Assert.Equal("PRODUCT_HAS_SALE", ex.ErrorCode);
    }

    [Fact]
    public async Task Excluir_peca_com_venda_id_retorna_PRODUCT_HAS_SALE()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (_, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Disponivel);

        var venda = new Domain.Entities.Venda(fx.EmpresaId, "VND-DEL", fx.UsuarioId, "Pix");
        fx.Db.Vendas.Add(venda);
        await fx.Db.SaveChangesAsync();

        peca.VendaId = venda.Id;
        await fx.Db.SaveChangesAsync();

        var handler = CriarPecasHandler(fx);

        var ex = await Assert.ThrowsAsync<BazarException>(() =>
            handler.Handle(new DeletePecaRequest(peca.Id), CancellationToken.None));

        Assert.Equal(409, ex.StatusCode);
        Assert.Equal("PRODUCT_HAS_SALE", ex.ErrorCode);
    }

    [Fact]
    public async Task Excluir_peca_disponivel_retorna_sucesso()
    {
        using var fx = new BazarTestFixture();
        var c = fx.CriarConsignante();
        var (_, peca) = fx.CriarSacolaComPeca(c.Id, BazarProdutoStatus.Disponivel);

        var handler = CriarPecasHandler(fx);
        await handler.Handle(new DeletePecaRequest(peca.Id), CancellationToken.None);

        var pecaDb = await fx.Db.BazarProdutos.FindAsync(peca.Id);
        Assert.NotNull(pecaDb);
        Assert.NotNull(pecaDb!.DeletadoEm);

        var auditoria = await fx.Db.AuditoriaAcoes
            .Where(a => a.Acao == "bazar.excluir.peca" && a.RecursoId == peca.Id.ToString())
            .ToListAsync();
        Assert.Single(auditoria);
    }

    private static BazarConsignantesHandlers CriarConsignantesHandler(BazarTestFixture fx) =>
        new(
            fx.Db,
            new FakeEmpresaContext(fx.EmpresaId),
            new BazarResumoService(fx.Db),
            new BazarAuditoriaService(
                fx.Db,
                new FakeEmpresaContext(fx.EmpresaId),
                new FakeHttpContextAccessor(fx.UsuarioId)));

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
