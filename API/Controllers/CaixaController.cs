using Microsoft.AspNetCore.Mvc;
using MediatR;
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
    }
}
