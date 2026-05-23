using CRMVEXA.Application.Constants;
using Xunit;

namespace CRMVEXA.Tests.Personalizacao;

public class ModuloCatalogoTests
{
    [Fact]
    public void Nao_pode_desativar_modulo_fixo()
    {
        var estado = ModuloCatalogo.Todos.ToDictionary(m => m.Key, _ => true);
        estado["inicio"] = false;

        var ex = Assert.Throws<ModuloValidacaoException>(() => ModuloCatalogo.ValidarModulos(estado));
        Assert.Equal("FIXED_MODULE", ex.ErrorCode);
    }

    [Fact]
    public void Nao_pode_desativar_produtos_com_vendas_ativa()
    {
        var estado = ModuloCatalogo.Todos.ToDictionary(m => m.Key, _ => true);
        estado["produtos"] = false;
        estado["bazar"] = false;

        var ex = Assert.Throws<ModuloValidacaoException>(() => ModuloCatalogo.ValidarModulos(estado));
        Assert.Equal("DEPENDENCY_VIOLATION", ex.ErrorCode);
        Assert.Contains("vendas", ex.BlockingModules ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void Pode_desativar_produtos_se_bazar_ativo_para_vendas()
    {
        var estado = ModuloCatalogo.Todos.ToDictionary(m => m.Key, _ => false);
        foreach (var def in ModuloCatalogo.Todos.Where(d => d.Fixed))
            estado[def.Key] = true;
        estado["produtos"] = false;
        estado["vendas"] = true;
        estado["bazar"] = true;
        estado["compras"] = false;
        estado["estoque"] = false;
        estado["catalogo"] = false;

        ModuloCatalogo.ValidarModulos(estado);
    }

    [Fact]
    public void Nao_pode_ativar_fiscal_sem_vendas()
    {
        var estado = ModuloCatalogo.Todos.ToDictionary(m => m.Key, _ => false);
        foreach (var def in ModuloCatalogo.Todos.Where(d => d.Fixed))
            estado[def.Key] = true;
        estado["fiscal"] = true;
        estado["vendas"] = false;

        var ex = Assert.Throws<ModuloValidacaoException>(() => ModuloCatalogo.ValidarModulos(estado));
        Assert.Equal("DEPENDENCY_VIOLATION", ex.ErrorCode);
    }
}
