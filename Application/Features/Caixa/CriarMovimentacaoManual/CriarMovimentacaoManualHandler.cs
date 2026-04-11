using System.Globalization;
using System.Security.Claims;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace CRMVEXA.Application.Features.Caixa.CriarMovimentacaoManual;

public class CriarMovimentacaoManualHandler : IRequestHandler<CriarMovimentacaoManualRequest, CriarMovimentacaoManualResponse>
{
    private static readonly HashSet<string> FormasEntradaValidas =
        new(StringComparer.OrdinalIgnoreCase) { "dinheiro", "credito", "debito", "pix" };

    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CriarMovimentacaoManualHandler(
        AppDbContext context,
        IEmpresaContext empresaContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _empresaContext = empresaContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CriarMovimentacaoManualResponse> Handle(CriarMovimentacaoManualRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

        var empresaId = _empresaContext.EmpresaId;
        var tipoNorm = (request.Tipo ?? string.Empty).Trim().ToLowerInvariant();
        if (tipoNorm is not ("entrada_lucro" or "saida_despesa"))
            throw new InvalidOperationException("tipo deve ser entrada_lucro ou saida_despesa.");

        if (request.Valor <= 0)
            throw new InvalidOperationException("valor deve ser maior que zero.");

        var descricao = (request.Descricao ?? string.Empty).Trim();
        var categoria = (request.Categoria ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(descricao))
            throw new InvalidOperationException("descricao é obrigatória.");
        if (string.IsNullOrEmpty(categoria))
            throw new InvalidOperationException("categoria é obrigatória.");

        var dataStr = (request.Data ?? string.Empty).Trim();
        DateTime dataLancamentoUtc;
        if (DateOnly.TryParse(dataStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataOnly))
            dataLancamentoUtc = DateTime.SpecifyKind(dataOnly.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        else if (DateTime.TryParse(dataStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dtParsed))
            dataLancamentoUtc = DateTime.SpecifyKind(dtParsed.Date, DateTimeKind.Utc);
        else
            throw new InvalidOperationException("data inválida. Use o formato YYYY-MM-DD.");

        string tipoResumo = tipoNorm == "entrada_lucro" ? "entrada_manual" : "saida_manual";

        string? formaPagamento = null;
        if (tipoNorm == "entrada_lucro")
        {
            var fp = (request.FormaPagamento ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(fp))
                fp = "dinheiro";
            if (!FormasEntradaValidas.Contains(fp))
                throw new InvalidOperationException("formaPagamento deve ser dinheiro, credito, debito ou pix.");
            formaPagamento = fp;
        }

        var usuarioId = ObterUsuarioIdDoToken();

        var entidade = new MovimentacaoCaixaManual(
            empresaId,
            usuarioId > 0 ? usuarioId : null,
            tipoResumo,
            request.Valor,
            descricao,
            categoria,
            dataLancamentoUtc,
            string.IsNullOrWhiteSpace(request.Observacoes) ? null : request.Observacoes.Trim(),
            formaPagamento);

        _context.MovimentacoesCaixaManuais.Add(entidade);
        await _context.SaveChangesAsync(cancellationToken);

        var entrada = tipoResumo == "entrada_manual";
        var valorAbs = Math.Abs(entidade.Valor);
        return new CriarMovimentacaoManualResponse
        {
            Id = entidade.Id,
            Tipo = tipoResumo,
            Valor = entrada ? valorAbs : -valorAbs,
            Descricao = entidade.Descricao,
            Categoria = entidade.Categoria,
            Data = entidade.DataLancamento,
            Observacoes = entidade.Observacoes,
            FormaPagamento = entidade.FormaPagamento,
            Natureza = entrada ? "entrada" : "saida"
        };
    }

    private int ObterUsuarioIdDoToken()
    {
        var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}
