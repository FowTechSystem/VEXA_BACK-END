namespace CRMVEXA.Domain.Entities;

public class Estoque
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public int EstoqueMinimo { get; set; }
    public string? LocalizacaoEstoque { get; set; }

    public Produto Produto { get; set; } = null!;
    public Empresa Empresa { get; set; } = null!;

    private Estoque() { }

    public Estoque(int empresaId, int produtoId, int quantidade = 0, int estoqueMinimo = 1, string? localizacaoEstoque = null)
    {
        EmpresaId = empresaId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
        EstoqueMinimo = estoqueMinimo;
        LocalizacaoEstoque = localizacaoEstoque;
    }

    public void AtualizarQuantidade(int quantidade) => Quantidade += quantidade;
    public void DefinirEstoqueMinimo(int minimo) => EstoqueMinimo = minimo;
}
