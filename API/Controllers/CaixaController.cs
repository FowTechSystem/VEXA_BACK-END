using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Features.Caixa.CriarMovimentacaoManual;
using CRMVEXA.Application.Features.Caixa.ExcluirMovimentacaoManual;
using CRMVEXA.Application.Features.Caixa.GetResumo;

namespace CRMVEXA.API.Controllers
{
    /// <summary>
    /// Resumo do Caixa (entradas, saídas, saldo e movimentações unificadas).
    /// Rota: api/Caixa (singular para coincidir com o front: api/caixa/resumo).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CaixaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CaixaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retorna resumo do caixa: totalEntradas (vendas), totalSaidas (compras), saldoAtual e movimentacoes do período.
        /// </summary>
        [HttpGet("resumo")]
        public async Task<ActionResult<GetCaixaResumoResponse>> Resumo(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                var request = new GetCaixaResumoRequest
                {
                    DataInicio = dataInicio,
                    DataFim = dataFim
                };
                var resultado = await _mediator.Send(request);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        /// <summary>Lançamento manual de lucro (entrada) ou despesa (saída).</summary>
        [HttpPost("movimentacoes/manual")]
        public async Task<ActionResult<CriarMovimentacaoManualResponse>> CriarMovimentacaoManual(
            [FromBody] CriarMovimentacaoManualRequest? request)
        {
            if (request == null)
                return BadRequest(new { sucesso = false, message = "Corpo JSON obrigatório.", mensagem = "Corpo JSON obrigatório." });

            try
            {
                var resultado = await _mediator.Send(request);
                return StatusCode(StatusCodes.Status201Created, resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { sucesso = false, message = ex.Message, mensagem = ex.Message });
            }
        }

        /// <summary>Remove apenas lançamento manual (não venda/compra).</summary>
        [HttpDelete("movimentacoes/{id:int}")]
        public async Task<IActionResult> ExcluirMovimentacaoManual(int id)
        {
            try
            {
                await _mediator.Send(new ExcluirMovimentacaoManualRequest { Id = id });
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { sucesso = false, message = "Movimentação manual não encontrada.", mensagem = "Movimentação manual não encontrada." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { sucesso = false, message = ex.Message, mensagem = ex.Message });
            }
        }
    }
}
