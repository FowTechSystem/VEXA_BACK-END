using System.Collections.Generic;
using System.Linq;

namespace CRMVEXA.Application.Features.Produtos;

public static class ProdutoListaStringHelper
{
    public static List<string> Normalizar(IEnumerable<string>? itens) =>
        itens?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList() ?? new List<string>();
}
