namespace CRMVEXA.Application.Features.Produtos;

public static class ProdutoPromocaoHelper
{
    /// <summary>Alinha com o front: preço &lt;= 0 ou ausente → sem promoção.</summary>
    public static (decimal? PrecoPromocional, bool EmPromocao) NormalizarPromocao(decimal? precoPromocional)
    {
        if (!precoPromocional.HasValue || precoPromocional.Value <= 0)
            return (null, false);
        return (precoPromocional.Value, true);
    }

    public static string? NormalizarObservacoes(string? observacoes)
    {
        if (string.IsNullOrWhiteSpace(observacoes))
            return null;
        return observacoes.Trim();
    }
}
