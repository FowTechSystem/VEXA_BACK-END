namespace CRMVEXA.Domain.Entities
{
    public class MovimentacaoEstoque
    {
        public int Id { get; private set; }
        public int ProdutoId { get; private set; }
        public string TipoMovimentacao { get; private set; } // Entrada, Saída, Ajuste
        public int Quantidade { get; private set; }
        public string Motivo { get; private set; }
        public int UsuarioId { get; private set; }
        public DateTime DataHora { get; private set; }

        private MovimentacaoEstoque() { }

        public MovimentacaoEstoque(int produtoId, string tipoMovimentacao, int quantidade, string motivo, int usuarioId)
        {
            ProdutoId = produtoId;
            TipoMovimentacao = tipoMovimentacao;
            Quantidade = quantidade;
            Motivo = motivo;
            UsuarioId = usuarioId;
            DataHora = DateTime.UtcNow;
        }
    }
}
