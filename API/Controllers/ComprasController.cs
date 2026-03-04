using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Features.Compras.CreateCompra;
using CRMVEXA.Application.Features.Compras.GetCompra;
using CRMVEXA.Application.Features.Compras.GetListaCompras;
using CRMVEXA.Application.Features.Compras.DeleteCompra;
using CRMVEXA.Application.Features.Compras.UpdateCompraStatus;

namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ComprasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<GetListaComprasResponse>> Listar(
            [FromQuery] string? valor,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            try
            {
                var request = new GetListaComprasRequest
                {
                    Valor = valor,
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
        public async Task<ActionResult<GetCompraResponse>> ObterPorId(int id)
        {
            try
            {
                var resultado = await _mediator.Send(new GetCompraRequest { Id = id });
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar([FromBody] CreateCompraRequest request)
        {
            try
            {
                var id = await _mediator.Send(request);
                return Ok(new { sucesso = true, id, mensagem = "Compra criada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<UpdateCompraStatusResponse>> AtualizarStatus([FromRoute] int id, [FromBody] UpdateCompraStatusRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Status))
                    return BadRequest(new { sucesso = false, mensagem = "Status é obrigatório" });
                request.Id = id;
                var resultado = await _mediator.Send(request);
                if (!resultado.Sucesso)
                    return BadRequest(resultado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar([FromRoute] int id)
        {
            try
            {
                await _mediator.Send(new DeleteCompraRequest { Id = id });
                return Ok(new { sucesso = true, mensagem = "Compra deletada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}
