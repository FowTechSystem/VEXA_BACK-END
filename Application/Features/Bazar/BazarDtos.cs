namespace CRMVEXA.Application.Features.Bazar;

public class BazarResumoResponse
{
    public decimal Pendente { get; set; }
    public int EmEstoque { get; set; }
    public int Vendidas { get; set; }
    public int Devolvidas { get; set; }
}

public class BazarConsignanteResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string? Cpf { get; set; }
    public string? Observacao { get; set; }
    public string CriadoEm { get; set; } = "";
    public int? SacolasCount { get; set; }
    public BazarResumoResponse? Resumo { get; set; }
}

public class BazarSacolaResponse
{
    public int Id { get; set; }
    public int ConsignanteId { get; set; }
    public string DataCadastro { get; set; } = "";
    public string? Observacao { get; set; }
    public int? Total { get; set; }
    public int? Disponiveis { get; set; }
    public int? Vendidas { get; set; }
    public int? Devolvidas { get; set; }
    public decimal? ValorVenda { get; set; }
    public decimal? ValorRepasse { get; set; }
    public bool? PodeDevolverTudo { get; set; }
    public List<BazarPecaIdResponse>? Pecas { get; set; }
}

public class BazarPecaIdResponse
{
    public int Id { get; set; }
}

public class BazarFotoResponse
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public int Ordem { get; set; }
}

public class BazarPecaResponse
{
    public int Id { get; set; }
    public int ConsignanteId { get; set; }
    public int SacolaId { get; set; }
    public string Descricao { get; set; } = "";
    public string? Tamanho { get; set; }
    public string? Cor { get; set; }
    public decimal ValorCompra { get; set; }
    public decimal ValorVenda { get; set; }
    public List<BazarFotoResponse> Fotos { get; set; } = new();
    public string Status { get; set; } = "";
    public string DataEntrada { get; set; } = "";
    public string? DataVenda { get; set; }
    public string? DataDevolucao { get; set; }
    public int? VendaId { get; set; }
}

public class BazarRepasseConsignanteRef
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Telefone { get; set; } = "";
}

public class BazarPecaPendenteResponse
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public string? Tamanho { get; set; }
    public string? Cor { get; set; }
    public decimal ValorCompra { get; set; }
    public string DataVenda { get; set; } = "";
}

public class BazarRepassePendenteGrupoResponse
{
    public BazarRepasseConsignanteRef Consignante { get; set; } = new();
    public List<BazarPecaPendenteResponse> Pecas { get; set; } = new();
    public decimal Total { get; set; }
}

public class BazarRepasseResponse
{
    public int Id { get; set; }
    public int ConsignanteId { get; set; }
    public BazarRepasseConsignanteRef? Consignante { get; set; }
    public string Data { get; set; } = "";
    public decimal Valor { get; set; }
    public string? FormaPagamento { get; set; }
    public string Status { get; set; } = "";
    public List<int> PecasIds { get; set; } = new();
    public string? Observacao { get; set; }
    public List<BazarPecaPendenteResponse>? Pecas { get; set; }
}
