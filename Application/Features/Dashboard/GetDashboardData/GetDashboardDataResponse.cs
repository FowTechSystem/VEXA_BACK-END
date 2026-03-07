namespace CRMVEXA.Application.Features.Dashboard.GetDashboardData;

/// <summary>
/// Resposta agregada do Dashboard. Todos os valores são referentes ao mês/ano selecionado,
/// exceto FaturamentoMensal (por mês do ano) e EstoqueCritico (sempre atual).
/// </summary>
public class GetDashboardDataResponse
{
    public int Mes { get; set; }
    public int Ano { get; set; }

    /// <summary>Lucro líquido do mês: soma de (precoVenda - precoCusto) * quantidade em todos os itens vendidos.</summary>
    public decimal FaturamentoMesLucro { get; set; }

    /// <summary>Quantidade de vendas realizadas no mês (exclui canceladas).</summary>
    public int TotalVendas { get; set; }

    /// <summary>Clientes que compraram neste mês e NUNCA tinham comprado antes.</summary>
    public int NovosClientes { get; set; }

    /// <summary>Valor bruto total das vendas do mês (para metas e faturamento).</summary>
    public decimal FaturamentoMesBruto { get; set; }

    /// <summary>Ticket médio = FaturamentoMesBruto / TotalVendas.</summary>
    public decimal TicketMedio { get; set; }

    /// <summary>Faturamento bruto por mês do ano selecionado (para gráfico).</summary>
    public List<FaturamentoMensalItem> FaturamentoMensal { get; set; } = new();

    /// <summary>Valor vendido por categoria no mês.</summary>
    public List<VendasPorCategoriaItem> VendasPorCategoria { get; set; } = new();

    /// <summary>Top 4 produtos mais vendidos no mês (por quantidade).</summary>
    public List<ProdutoMaisVendidoItem> ProdutosMaisVendidos { get; set; } = new();

    /// <summary>Produtos com estoque abaixo do mínimo (menos de 10% do mínimo ou abaixo do mínimo).</summary>
    public List<EstoqueCriticoItem> EstoqueCritico { get; set; } = new();

    /// <summary>Top 3 vendedores do mês (por valor total vendido).</summary>
    public List<TopVendedorItem> TopVendedores { get; set; } = new();

    /// <summary>Distribuição do valor das vendas por forma de pagamento no mês.</summary>
    public List<FormaPagamentoItem> FormasPagamento { get; set; } = new();
}

public class FaturamentoMensalItem
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal ValorBruto { get; set; }
}

public class VendasPorCategoriaItem
{
    public int CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public int QuantidadeVendida { get; set; }
    public decimal ValorVendido { get; set; }
}

public class ProdutoMaisVendidoItem
{
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int QuantidadeVendida { get; set; }
    public decimal ValorVendido { get; set; }
}

public class EstoqueCriticoItem
{
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int QuantidadeAtual { get; set; }
    public int EstoqueMinimo { get; set; }
    public string Status { get; set; } = string.Empty; // "CRITICO" quando abaixo do mínimo
}

public class TopVendedorItem
{
    public int UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeVendas { get; set; }
    public decimal ValorTotalVendido { get; set; }
}

public class FormaPagamentoItem
{
    public string FormaPagamento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Percentual { get; set; }
}
