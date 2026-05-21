using CRMVEXA.Application.Features.Clientes;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CRMVEXA.Application.Features.Produtos;

/// <summary>Até 5 fotos em bytea — mesmo contrato de decode que cliente/usuário.</summary>
public static class ProdutoFotoSlotHelper
{
    private static readonly string[] AdditionalSlots =
    [
        nameof(Produto.Foto2),
        nameof(Produto.Foto3),
        nameof(Produto.Foto4),
        nameof(Produto.Foto5)
    ];

    public static byte[]? DecodificarCriacao(string? fotoBase64, string? legadoUrlOuBase64) =>
        ClienteFotoPayloadHelper.TryDecodeToBytes(fotoBase64 ?? legadoUrlOuBase64);

    /// <summary>
    /// Resolve a galeria adicional por ordem (2..5). Arrays têm prioridade sobre campos legados.
    /// </summary>
    public static byte[]?[] DecodificarAdicionaisCriacao(
        IList<string>? fotosAdicionaisUrls,
        IList<string>? fotosAdicionaisBase64,
        params string?[] legacyBySlot)
    {
        if ((fotosAdicionaisUrls?.Count ?? 0) == 0 && (fotosAdicionaisBase64?.Count ?? 0) == 0)
        {
            return
            [
                ClienteFotoPayloadHelper.TryDecodeToBytes(legacyBySlot.ElementAtOrDefault(0)),
                ClienteFotoPayloadHelper.TryDecodeToBytes(legacyBySlot.ElementAtOrDefault(1)),
                ClienteFotoPayloadHelper.TryDecodeToBytes(legacyBySlot.ElementAtOrDefault(2)),
                ClienteFotoPayloadHelper.TryDecodeToBytes(legacyBySlot.ElementAtOrDefault(3))
            ];
        }

        var merged = BuildMergedPayload(fotosAdicionaisUrls, fotosAdicionaisBase64);
        var decoded = new byte[]?[AdditionalSlots.Length];
        for (var i = 0; i < decoded.Length; i++)
            decoded[i] = ClienteFotoPayloadHelper.TryDecodeToBytes(merged.ElementAtOrDefault(i));
        return decoded;
    }

    /// <summary>
    /// Quando arrays adicionais são enviados, substitui integralmente Foto2..Foto5 por ordem.
    /// Itens excedentes são ignorados.
    /// </summary>
    public static void AplicarAtualizacaoAdicionais(
        AppDbContext context,
        Produto produto,
        IList<string>? fotosAdicionaisUrls,
        IList<string>? fotosAdicionaisBase64)
    {
        if ((fotosAdicionaisUrls?.Count ?? 0) == 0 && (fotosAdicionaisBase64?.Count ?? 0) == 0
            && fotosAdicionaisUrls == null && fotosAdicionaisBase64 == null)
            return;

        var merged = BuildMergedPayload(fotosAdicionaisUrls, fotosAdicionaisBase64);
        for (var i = 0; i < AdditionalSlots.Length; i++)
        {
            var payload = merged.ElementAtOrDefault(i);
            if (string.IsNullOrWhiteSpace(payload))
                continue;

            var bytes = DecodificarObrigatorio(payload, $"imagem adicional (slot {i + 2})");
            Atribuir(produto, AdditionalSlots[i], bytes);
            context.Entry(produto).Property(AdditionalSlots[i]).IsModified = true;
        }
    }

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

        if (string.IsNullOrWhiteSpace(payload))
        {
            Atribuir(produto, propertyName, null);
            context.Entry(produto).Property(propertyName).IsModified = true;
            return;
        }

        var bytes = DecodificarObrigatorio(payload, "imagem principal (Foto1)");
        Atribuir(produto, propertyName, bytes);
        context.Entry(produto).Property(propertyName).IsModified = true;
    }

    /// <summary>Se Foto1 vazia e houver galeria, usa a primeira adicional como principal (lista/thumbnail).</summary>
    public static void GarantirFotoPrincipal(Produto produto, AppDbContext? context = null)
    {
        if (produto.Foto1 is { Length: > 0 })
            return;

        foreach (var slot in AdditionalSlots)
        {
            var bytes = ObterBytes(produto, slot);
            if (bytes is not { Length: > 0 })
                continue;

            produto.Foto1 = bytes;
            if (context != null)
                context.Entry(produto).Property(nameof(Produto.Foto1)).IsModified = true;
            return;
        }
    }

    private static byte[] DecodificarObrigatorio(string payload, string descricao)
    {
        var bytes = ClienteFotoPayloadHelper.TryDecodeToBytes(payload);
        if (bytes is { Length: > 0 })
            return bytes;

        throw new InvalidOperationException(
            $"Não foi possível processar a {descricao}. A imagem pode estar truncada ou em formato inválido. " +
            "Prefira JPG/PNG ou envie pelo botão Escolher arquivo.");
    }

    private static byte[]? ObterBytes(Produto p, string name) => name switch
    {
        nameof(Produto.Foto2) => p.Foto2,
        nameof(Produto.Foto3) => p.Foto3,
        nameof(Produto.Foto4) => p.Foto4,
        nameof(Produto.Foto5) => p.Foto5,
        _ => null
    };

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

    private static List<string?> BuildMergedPayload(
        IList<string>? urls,
        IList<string>? base64)
    {
        var max = Math.Max(urls?.Count ?? 0, base64?.Count ?? 0);
        var merged = new List<string?>(max);
        for (var i = 0; i < max; i++)
            merged.Add(urls?.ElementAtOrDefault(i) ?? base64?.ElementAtOrDefault(i));
        return merged;
    }
}
