using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Features.Estoque.GetListaEstoque;
using CRMVEXA.Application.Features.Estoque.GetListaMovimentacoes;

namespace CRMVEXA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstoqueController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstoqueController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Lista produtos com quantidade em estoque. Somente leitura - não há lançamento manual.</summary>
    [HttpGet]
    public async Task<ActionResult<GetListaEstoqueResponse>> Listar(
        [FromQuery] string? valor,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 50)
    {
        try
        {
            var resultado = await _mediator.Send(new GetListaEstoqueRequest
            {
                Valor = valor,
                Pagina = pagina,
                Tamanho = tamanho
            });
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }

    /// <summary>Histórico de movimentações (entradas por compra lançada/recebida, saídas por venda, etc.).</summary>
    [HttpGet("movimentacoes")]
    public async Task<ActionResult<GetListaMovimentacoesResponse>> Movimentacoes(
        [FromQuery] int? produtoId,
        [FromQuery] string? tipo,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 50)
    {
        try
        {
            var resultado = await _mediator.Send(new GetListaMovimentacoesRequest
            {
                ProdutoId = produtoId,
                Tipo = tipo,
                Pagina = pagina,
                Tamanho = tamanho
            });
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }
}
