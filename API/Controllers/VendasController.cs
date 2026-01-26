using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<ActionResult<GetListaVendasResponse>> Listar()
        {
            try
            {
                var resultado = await _mediator.Send(new GetListaVendasRequest());
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

        [HttpPost]
        public async Task<ActionResult<int>> Criar([FromBody] CreateVendaRequest request)
        {
            try
            {
                var id = await _mediator.Send(request);
                return Ok(new { sucesso = true, id, mensagem = "Venda criada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
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
