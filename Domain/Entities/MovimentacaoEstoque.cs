namespace CRMVEXA.Domain.Entities
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int ProdutoId { get; set; }
        public string TipoMovimentacao { get; set; } // Entrada, Saída
        public int Quantidade { get; set; }
        public string Motivo { get; set; }
        public string? Observacoes { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataHora { get; set; }
        public int? CompraId { get; set; }
        public int? VendaId { get; set; }

        private MovimentacaoEstoque() { }

        public MovimentacaoEstoque(int empresaId, int produtoId, string tipoMovimentacao, int quantidade, string motivo, int usuarioId, int? compraId = null, int? vendaId = null)
        {
            EmpresaId = empresaId;
            ProdutoId = produtoId;
            TipoMovimentacao = tipoMovimentacao;
            Quantidade = quantidade;
            Motivo = motivo;
            UsuarioId = usuarioId;
            DataHora = DateTime.UtcNow;
            CompraId = compraId;
            VendaId = vendaId;
        }
    }
}
