using CRMVEXA.Application.Features.Clientes;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos;

/// <summary>Até 5 fotos em bytea — mesmo contrato de decode que cliente/usuário.</summary>
public static class ProdutoFotoSlotHelper
{
    public static byte[]? DecodificarCriacao(string? fotoBase64, string? legadoUrlOuBase64) =>
        ClienteFotoPayloadHelper.TryDecodeToBytes(fotoBase64 ?? legadoUrlOuBase64);

    public static void AplicarAtualizacaoSlot(
        AppDbContext context,
        Produto produto,
        string propertyName,
        string? fotoUrl,
        string? fotoBase64,
        string? legadoMesmoNome = null)
    {
        var payload = fotoUrl ?? fotoBase64 ?? legadoMesmoNome;
        if (payload == null)
            return;

        var bytes = ClienteFotoPayloadHelper.TryDecodeToBytes(payload);
        if (bytes != null)
        {
            Atribuir(produto, propertyName, bytes);
            context.Entry(produto).Property(propertyName).IsModified = true;
        }
        else if (string.IsNullOrWhiteSpace(payload))
        {
            Atribuir(produto, propertyName, null);
            context.Entry(produto).Property(propertyName).IsModified = true;
        }
    }

    private static void Atribuir(Produto p, string name, byte[]? value)
    {
        switch (name)
        {
            case nameof(Produto.Foto1): p.Foto1 = value; break;
            case nameof(Produto.Foto2): p.Foto2 = value; break;
            case nameof(Produto.Foto3): p.Foto3 = value; break;
            case nameof(Produto.Foto4): p.Foto4 = value; break;
            case nameof(Produto.Foto5): p.Foto5 = value; break;
        }
    }
}
