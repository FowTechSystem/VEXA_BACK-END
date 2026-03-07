using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CRMVEXA.Application.Features.Vendas.CreateVenda;
using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Application.Features.Vendas.GetListaVendas;
using CRMVEXA.Application.Features.Vendas.CancelarVenda;

namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VendasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lista vendas (Histórico). Aceita dataInicio/dataFim (ISO 8601) ou mes/ano.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<GetListaVendasResponse>> Listar(
            [FromQuery] string? valor,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] int? mes,
            [FromQuery] int? ano,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 500)
        {
            try
            {
                var request = new GetListaVendasRequest
                {
                    Valor = valor,
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    Mes = mes,
                    Ano = ano,
                    Pagina = pagina,
                    Tamanho = tamanho
                };
                var resultado = await _mediator.Send(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetVendaResponse>> ObterPorId(int id)
        {
            try
            {
                var resultado = await _mediator.Send(new GetVendaRequest { Id = id });
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Cria venda (spec Caixa). Retorna 201 com o objeto criado.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateVendaResponse>> Criar([FromBody] CreateVendaRequest? request)
        {
            if (request == null)
                return BadRequest(new { sucesso = false, mensagem = "Corpo da requisição inválido ou vazio. Envie JSON com itens, valorTotal, formaPagamento." });

            if (request.Itens == null || request.Itens.Count == 0)
                return BadRequest(new { sucesso = false, mensagem = "A venda deve ter pelo menos um item (itens não pode ser vazio)." });

            try
            {
                var resultado = await _mediator.Send(request);
                return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ObterMensagemErro(ex) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ObterMensagemErro(ex) });
            }
        }

        private static string ObterMensagemErro(Exception ex)
        {
            var e = ex;
            while (e?.InnerException != null) e = e.InnerException;
            return e?.Message ?? ex.Message;
        }

        [HttpPost("{id}/cancelar")]
        public async Task<ActionResult> Cancelar([FromRoute] int id, [FromBody] CancelarVendaRequest request)
        {
            try
            {
                request.Id = id;
                await _mediator.Send(request);
                return Ok(new { sucesso = true, mensagem = "Venda cancelada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}
