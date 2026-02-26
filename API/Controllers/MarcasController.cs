using CRMVEXA.Application.Features.Marcas.CreateMarca;
using CRMVEXA.Application.Features.Marcas.DeleteMarca;
using CRMVEXA.Application.Features.Marcas.GetMarca;
using CRMVEXA.Application.Features.Marcas.GetListaMarcas;
using CRMVEXA.Application.Features.Marcas.UpdateMarca;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarcasController : ControllerBase
{
    private readonly IMediator _mediator;

    public MarcasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetListaMarcasResponse>> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _mediator.Send(new GetListaMarcasRequest(), cancellationToken);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetMarcaResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _mediator.Send(new GetMarcaRequest { Id = id }, cancellationToken);
            if (resultado == null)
                return NotFound(new { sucesso = false, mensagem = "Marca não encontrada.", message = "Marca não encontrada." });
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateMarcaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nome))
                return BadRequest(new { sucesso = false, mensagem = "Nome é obrigatório.", message = "Nome é obrigatório." });
            var id = await _mediator.Send(request, cancellationToken);
            return Ok(new { sucesso = true, id, mensagem = "Marca criada com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateMarcaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nome))
                return BadRequest(new { sucesso = false, mensagem = "Nome é obrigatório.", message = "Nome é obrigatório." });
            request.Id = id;
            var resultado = await _mediator.Send(request, cancellationToken);
            if (!resultado.Sucesso)
                return NotFound(new { sucesso = false, mensagem = resultado.Mensagem, message = resultado.Mensagem });
            return Ok(new { sucesso = true, mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _mediator.Send(new DeleteMarcaRequest { Id = id }, cancellationToken);
            if (!resultado.Sucesso)
                return BadRequest(new { sucesso = false, mensagem = resultado.Mensagem, message = resultado.Mensagem });
            return Ok(new { sucesso = true, mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }
}
