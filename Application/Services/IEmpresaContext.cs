namespace CRMVEXA.Application.Services;

/// <summary>
/// Fornece o EmpresaId do tenant atual (extraído do JWT / contexto da requisição).
/// </summary>
public interface IEmpresaContext
{
    /// <summary>Id da empresa do usuário logado. Lança se não estiver disponível.</summary>
    int EmpresaId { get; }

    /// <summary>True se o contexto tem um EmpresaId válido.</summary>
    bool TemEmpresa { get; }
}
