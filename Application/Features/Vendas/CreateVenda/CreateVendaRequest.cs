using MediatR;

namespace CRMVEXA.Application.Features.Vendas.CreateVenda
{
    /// <summary>
    /// Request alinhado à spec do Caixa (frontend).
    /// Aceita: itens, valorTotal, formaPagamento, desconto, clienteId (opcional), observacoes (opcional).
    /// CodigoVenda é gerado no backend. UsuarioId obtido do token se não informado.
    /// </summary>
    public class CreateVendaRequest : IRequest<CreateVendaResponse>
    {
        public List<ItemVendaRequest> Itens { get; set; } = new();
        public decimal ValorTotal { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        /// <summary>Valor do desconto. Aceita null ou omitido; a coluna no banco permite NULL.</summary>
        public decimal? Desconto { get; set; }
        public int? ClienteId { get; set; }
        public string? Observacoes { get; set; }
        /// <summary>Opcional. Se não informado, usa o usuário do token JWT.</summary>
        public int? UsuarioId { get; set; }
        /// <summary>Opcional. Se não informado, gera automaticamente (ex: VND-20260213-143000).</summary>
        public string? CodigoVenda { get; set; }
    }

    public class ItemVendaRequest
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
