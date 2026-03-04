namespace CRMVEXA.Application.Features.Estoque.GetListaMovimentacoes;

public class GetListaMovimentacoesResponse
{
    public List<MovimentacaoResponse> Movimentacoes { get; set; } = new();
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public int TamanhoPagina { get; set; }
}

public class MovimentacaoResponse
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // entrada | saida
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string? ProdutoSku { get; set; }
    public int Quantidade { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public DateTime Data { get; set; }
    public DateTime Timestamp { get => Data; set => Data = value; }
}
