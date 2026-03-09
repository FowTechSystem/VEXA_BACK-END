using System.Security.Claims;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Condicional.CreateCondicional;

public class CreateCondicionalHandler : IRequestHandler<CreateCondicionalRequest, CreateCondicionalResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCondicionalHandler(AppDbContext context, IEmpresaContext empresaContext, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _empresaContext = empresaContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CreateCondicionalResponse> Handle(CreateCondicionalRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

        var empresaId = _empresaContext.EmpresaId;
        var usuarioId = ObterUsuarioIdDoToken();
        if (usuarioId <= 0)
            throw new InvalidOperationException("Usuário não identificado. Faça login novamente.");

        var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId && u.EmpresaId == empresaId, cancellationToken);
        if (!usuarioExiste)
            throw new InvalidOperationException($"Usuário com ID {usuarioId} não encontrado para esta empresa.");

        var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == request.ClienteId && c.EmpresaId == empresaId, cancellationToken);
        if (!clienteExiste)
            throw new InvalidOperationException($"Cliente com ID {request.ClienteId} não encontrado.");

        if (request.Itens == null || request.Itens.Count == 0)
            throw new InvalidOperationException("O condicional deve ter pelo menos um item.");

        decimal valorTotalCalculado = 0;
        var condicional = new Domain.Entities.Condicional(empresaId, request.ClienteId, usuarioId, request.ValorTotal);

        foreach (var itemReq in request.Itens)
        {
            var produto = await _context.Produtos
                .Where(p => p.EmpresaId == empresaId)
                .Include(p => p.Estoque)
                .FirstOrDefaultAsync(p => p.Id == itemReq.ProdutoId, cancellationToken);
            if (produto == null)
                throw new InvalidOperationException($"Produto com ID {itemReq.ProdutoId} não encontrado.");
            if (produto.Estoque == null)
                throw new InvalidOperationException($"Produto '{produto.Nome}' não possui estoque cadastrado.");
            if (produto.Estoque.Quantidade < itemReq.Quantidade)
                throw new InvalidOperationException($"Estoque insuficiente para {produto.Nome}. Disponível: {produto.Estoque.Quantidade}.");

            var item = new ItemCondicional(itemReq.ProdutoId, itemReq.Quantidade, itemReq.PrecoUnitario);
            condicional.AdicionarItem(item);
            valorTotalCalculado += item.Subtotal;

            produto.Estoque.AtualizarQuantidade(-itemReq.Quantidade);

            var movimentacao = new MovimentacaoEstoque(
                empresaId,
                itemReq.ProdutoId,
                "Saída",
                itemReq.Quantidade,
                "Condicional",
                usuarioId,
                compraId: null,
                vendaId: null);
            _context.MovimentacoesEstoque.Add(movimentacao);
        }

        condicional.ValorTotal = valorTotalCalculado;

        _context.Condicionais.Add(condicional);
        await _context.SaveChangesAsync(cancellationToken);

        var produtoIds = condicional.Itens.Select(i => i.ProdutoId).ToList();
        var produtos = await _context.Produtos
            .Where(p => produtoIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var itensResp = condicional.Itens.Select(i =>
        {
            produtos.TryGetValue(i.ProdutoId, out var p);
            return new CreateCondicionalItemResponse
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                ProdutoNome = p?.Nome
            };
        }).ToList();

        return new CreateCondicionalResponse
        {
            Id = condicional.Id,
            Data = condicional.Data,
            ClienteId = condicional.ClienteId,
            ValorTotal = condicional.ValorTotal,
            Status = condicional.Status,
            Itens = itensResp
        };
    }

    private int ObterUsuarioIdDoToken()
    {
        var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}
