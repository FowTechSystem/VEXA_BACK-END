using System.Linq;

namespace CRMVEXA.Domain.Entities;

/// <summary>
/// Condicional: produtos reservados para cliente, contam como venda até devolução.
/// </summary>
public class Condicional
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Data { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = "pendente"; // pendente | devolvido

    public Empresa Empresa { get; set; } = null!;
    public Cliente Cliente { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;

    private List<ItemCondicional> _itens = new();
    public IReadOnlyCollection<ItemCondicional> Itens => _itens.AsReadOnly();

    private Condicional() { }

    public Condicional(int empresaId, int clienteId, int usuarioId, decimal valorTotal)
    {
        EmpresaId = empresaId;
        ClienteId = clienteId;
        UsuarioId = usuarioId;
        ValorTotal = valorTotal;
        Data = DateTime.UtcNow;
    }

    public void AdicionarItem(ItemCondicional item)
    {
        _itens.Add(item);
    }

    public void MarcarDevolvido() => Status = "devolvido";
}
