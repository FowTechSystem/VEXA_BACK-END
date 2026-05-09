using System;
using System.Security.Claims;
using CRMVEXA.Application.Constants;
using CRMVEXA.Application.Features.Compras.CreateCompra;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace CRMVEXA.Application.Features.Compras.CreateCompra
{
    public class CreateCompraHandler : IRequestHandler<CreateCompraRequest, int>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEntradaEstoquePorCompraService _entradaEstoque;

        public CreateCompraHandler(
            AppDbContext context,
            IEmpresaContext empresaContext,
            IHttpContextAccessor httpContextAccessor,
            IEntradaEstoquePorCompraService entradaEstoque)
        {
            _context = context;
            _empresaContext = empresaContext;
            _httpContextAccessor = httpContextAccessor;
            _entradaEstoque = entradaEstoque;
        }

        public async Task<int> Handle(CreateCompraRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;

            // Validar fornecedor existe
            var fornecedor = await _context.Fornecedores
                .FirstOrDefaultAsync(f => f.EmpresaId == empresaId && f.Id == request.FornecedorId, cancellationToken);
            if (fornecedor == null)
                throw new InvalidOperationException($"Fornecedor com ID {request.FornecedorId} não encontrado. Selecione um fornecedor cadastrado.");

            if (request.Itens == null || request.Itens.Count == 0)
                throw new InvalidOperationException("A compra deve conter pelo menos um item.");

            var usuarioId = request.UsuarioId ?? ObterUsuarioIdDoToken();
            if (usuarioId <= 0)
                throw new InvalidOperationException("Usuário não identificado. Faça login novamente.");

            var codigoCompra = $"CMP-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
            var status = NormalizarStatus(request.Status);
            var dataPedido = request.DataPedido ?? DateTime.UtcNow;
            var valorTotal = request.ValorTotal;

            var compra = new Compra(empresaId, codigoCompra, request.FornecedorId, usuarioId, request.Observacoes);
            compra.DefinirStatusEDatas(status, dataPedido, request.DataEntregaPrevista, valorTotal);

            foreach (var itemRequest in request.Itens)
            {
                var produto = await _context.Produtos
                    .FirstOrDefaultAsync(p => p.EmpresaId == empresaId && p.Id == itemRequest.ProdutoId, cancellationToken);
                if (produto == null)
                    throw new InvalidOperationException($"Produto com ID {itemRequest.ProdutoId} não encontrado");

                var itemCompra = new ItemCompra(itemRequest.ProdutoId, itemRequest.Quantidade, itemRequest.PrecoUnitario);
                compra.AdicionarItem(itemCompra);
            }

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync(cancellationToken);

            // Entrada física só quando recebida (alinhado ao front Estoque.jsx + contrato de negócio).
            if (CompraEstoqueConstants.StatusDisparaEntradaFisica(compra.Status))
            {
                var motivo = $"Compra {compra.CodigoCompra} recebida";
                await _entradaEstoque.RegistrarEntradaAsync(empresaId, compra, usuarioId, motivo, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return compra.Id;
        }

        private static string NormalizarStatus(string? status)
        {
            var s = (status ?? "pendente").Trim().ToLowerInvariant();
            if (s is "pendente" or "aprovada" or "recebida" or "cancelada") return s;
            return "pendente";
        }

        private int ObterUsuarioIdDoToken()
        {
            var idStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }
    }
}
