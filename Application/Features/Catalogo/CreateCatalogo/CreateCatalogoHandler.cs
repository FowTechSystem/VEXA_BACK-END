using System.Security.Claims;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Catalogo.CreateCatalogo;

public class CreateCatalogoHandler : IRequestHandler<CreateCatalogoRequest, CreateCatalogoResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCatalogoHandler(AppDbContext context, IEmpresaContext empresaContext, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _empresaContext = empresaContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CreateCatalogoResponse> Handle(CreateCatalogoRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

        var empresaId = _empresaContext.EmpresaId;
        var criadorId = ObterUsuarioIdDoToken();
        if (criadorId <= 0)
            throw new InvalidOperationException("Usuário não identificado. Faça login novamente.");

        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == criadorId && u.EmpresaId == empresaId, cancellationToken);
        if (!usuarioExiste)
            throw new InvalidOperationException("Usuário não encontrado para esta empresa.");

        if (request.Produtos == null || request.Produtos.Count < 20)
            throw new InvalidOperationException("O catálogo deve ter no mínimo 20 produtos.");

        var whatsAppLimpo = new string(request.CriadorWhatsApp.Where(char.IsDigit).ToArray());
        if (string.IsNullOrEmpty(whatsAppLimpo) || whatsAppLimpo.Length < 10 || whatsAppLimpo.Length > 11)
            throw new InvalidOperationException("WhatsApp inválido. Informe DDD + número (10 ou 11 dígitos).");

        if (!string.IsNullOrWhiteSpace(request.Slug) && !System.Text.RegularExpressions.Regex.IsMatch(request.Slug, @"^[a-zA-Z0-9\-]+$"))
            throw new InvalidOperationException("Slug deve conter apenas letras, números e hífen.");

        if (!string.IsNullOrWhiteSpace(request.Slug))
        {
            var slugExiste = await _context.Catalogos
                .AnyAsync(c => c.EmpresaId == empresaId && c.Slug == request.Slug.Trim(), cancellationToken);
            if (slugExiste)
                throw new InvalidOperationException($"O slug '{request.Slug}' já está em uso para esta empresa.");
        }

        var empresa = await _context.Empresas.FindAsync(new object[] { empresaId }, cancellationToken);
        var criador = await _context.Usuarios.FindAsync(new object[] { criadorId }, cancellationToken);

        var catalogo = new Domain.Entities.Catalogo(empresaId, criadorId, whatsAppLimpo)
        {
            CriadorNome = request.CriadorNome?.Trim() ?? criador?.Nome,
            EmpresaNome = request.EmpresaNome?.Trim() ?? empresa?.Nome,
            Slug = string.IsNullOrWhiteSpace(request.Slug) ? null : request.Slug.Trim()
        };

        var produtoIds = request.Produtos.Select(p => p.ProdutoId).Distinct().ToList();
        var produtos = await _context.Produtos
            .Where(p => p.EmpresaId == empresaId && produtoIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var item in request.Produtos)
        {
            if (!produtos.TryGetValue(item.ProdutoId, out var produto))
                throw new InvalidOperationException($"Produto com ID {item.ProdutoId} não encontrado ou não pertence à empresa.");

            var nome = !string.IsNullOrWhiteSpace(item.Nome) ? item.Nome.Trim() : produto.Nome;
            var qtd = item.Quantidade < 1 ? 1 : item.Quantidade;
            var itemCat = new ItemCatalogo(item.ProdutoId, nome, item.Preco, qtd);
            catalogo.AdicionarProduto(itemCat);
        }

        _context.Catalogos.Add(catalogo);
        await _context.SaveChangesAsync(cancellationToken);

        var baseUrl = _httpContextAccessor.HttpContext?.Request.Scheme + "://" +
                      _httpContextAccessor.HttpContext?.Request.Host;
        var linkPublico = $"{baseUrl}/catalogo-publico/{catalogo.Id}";

        return new CreateCatalogoResponse
        {
            Id = catalogo.Id,
            LinkPublico = linkPublico,
            CriadoEm = catalogo.CriadoEm
        };
    }

    private int ObterUsuarioIdDoToken()
    {
        var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}
