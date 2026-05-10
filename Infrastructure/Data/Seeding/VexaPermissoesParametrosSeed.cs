using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.Data.Seeding;

/// <summary>
/// Catálogo global de permissões + parâmetros padrão por empresa (chaves do prompt VEXA ERP).
/// Executar após migrations (ex.: chamada pontual ou job de provisionamento).
/// </summary>
public static class VexaPermissoesParametrosSeed
{
    public static readonly IReadOnlyList<(string Modulo, string Acao, string Recurso, string Chave, string Rotulo)> CatalogoPermissoes =
    [
        ("vendas", "criar", "venda", "vendas.venda.criar", "Registrar venda"),
        ("vendas", "cancelar", "venda", "vendas.venda.cancelar", "Cancelar venda"),
        ("vendas", "desconto", "venda", "vendas.venda.desconto", "Aplicar desconto"),
        ("vendas", "ver_todas", "venda", "vendas.venda.ver_todas", "Ver todas as vendas"),
        ("caixa", "abrir", "caixa", "caixa.caixa.abrir", "Abrir/fechar caixa"),
        ("caixa", "sangria", "caixa", "caixa.caixa.sangria", "Sangria ou suprimento"),
        ("estoque", "cadastrar", "produto", "estoque.produto.cadastrar", "Cadastrar produto"),
        ("estoque", "editar_preco", "produto", "estoque.produto.editar_preco", "Editar preço de produto"),
        ("estoque", "entrada", "estoque", "estoque.estoque.entrada", "Registrar entrada de estoque"),
        ("estoque", "ajuste", "estoque", "estoque.estoque.ajuste", "Ajustar inventário"),
        ("financeiro", "ver", "relatorio", "financeiro.relatorio.ver", "Ver relatórios financeiros"),
        ("financeiro", "lancar", "conta", "financeiro.conta.lancar", "Lançar contas a pagar/receber"),
        ("admin", "gerenciar", "usuario", "admin.usuario.gerenciar", "Gerir utilizadores"),
        ("admin", "configurar", "sistema", "admin.sistema.configurar", "Alterar configurações do sistema"),
        ("admin", "gerenciar", "empresa", "admin.empresa.gerenciar", "Gerir empresas"),
    ];

    public static readonly IReadOnlyList<(string Chave, string Valor, string Tipo, string? Rotulo)> ParametrosPadrao =
    [
        ("desconto_maximo_vendedor", "10", "number", "% máximo sem aprovação"),
        ("prazo_cancelamento_minutos", "30", "number", "Minutos para cancelar sem aprovação"),
        ("formas_pagamento_ativas", """["credito","debito","pix","dinheiro"]""", "json", null),
        ("parcelamento_maximo", "12", "number", null),
        ("estoque_minimo_padrao", "5", "number", null),
        ("meta_faturamento_mes", "0", "number", null),
        ("meta_vendas_mes", "0", "number", null),
        ("meta_clientes_mes", "0", "number", null),
        ("condicional_prazo_dias", "3", "number", null),
        ("impressao_automatica", "true", "boolean", null),
        ("venda_abaixo_custo", "false", "boolean", "Permitir venda abaixo do custo"),
    ];

    /// <summary>Garante linhas em PermissoesCatalogo (idempotente por Chave).</summary>
    public static async Task EnsureCatalogoPermissoesAsync(AppDbContext db, CancellationToken ct = default)
    {
        foreach (var (modulo, acao, recurso, chave, rotulo) in CatalogoPermissoes)
        {
            var existe = await db.PermissoesCatalogo.AnyAsync(p => p.Chave == chave, ct);
            if (existe) continue;
            db.PermissoesCatalogo.Add(new PermissaoCatalogo
            {
                Modulo = modulo,
                Acao = acao,
                Recurso = recurso,
                Chave = chave,
                Rotulo = rotulo
            });
        }
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Insere parâmetros padrão para uma empresa se ainda não existirem.</summary>
    public static async Task EnsureParametrosPadraoEmpresaAsync(AppDbContext db, int empresaId, CancellationToken ct = default)
    {
        foreach (var (chave, valor, tipo, rotulo) in ParametrosPadrao)
        {
            var existe = await db.ParametrosSistema.AnyAsync(p => p.EmpresaId == empresaId && p.Chave == chave, ct);
            if (existe) continue;
            db.ParametrosSistema.Add(new ParametroSistema
            {
                EmpresaId = empresaId,
                Chave = chave,
                Valor = valor,
                Tipo = tipo,
                Rotulo = rotulo,
                AtualizadoEm = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync(ct);
    }
}
