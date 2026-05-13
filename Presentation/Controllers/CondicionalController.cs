using CRMVEXA.Application.Features.Condicional.CreateCondicional;
using CRMVEXA.Application.Features.Condicional.DevolverCondicional;
using CRMVEXA.Application.Features.Condicional.GetCondicional;
using CRMVEXA.Application.Features.Condicional.GetListaCondicionais;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicionalController : ControllerBase
{
    private readonly IMediator _mediator;

    public CondicionalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetListaCondicionaisResponse>> Listar(
        [FromQuery] bool? pendente,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 100,
        [FromQuery] int? clienteId = null,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        try
        {
            var request = new GetListaCondicionaisRequest
            {
                Pendente = pendente,
                Pagina = pagina,
                Tamanho = tamanho,
                ClienteId = clienteId,
                DataInicio = dataInicio,
                DataFim = dataFim
            };
            var resultado = await _mediator.Send(request);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> ObterPorId([FromRoute] int id)
    {
        try
        {
            var resultado = await _mediator.Send(new GetCondicionalRequest { Id = id });
            if (resultado == null)
                return NotFound(new { sucesso = false, mensagem = "Condicional não encontrado." });
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreateCondicionalResponse>> Criar([FromBody] CreateCondicionalRequest? request)
    {
        if (request == null)
            return BadRequest(new { sucesso = false, mensagem = "Corpo da requisição inválido ou vazio. Envie JSON com clienteId, itens e valorTotal." });

        try
        {
            var resultado = await _mediator.Send(request);
            return StatusCode(201, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            var inner = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new { sucesso = false, mensagem = $"Erro ao salvar: {inner}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpPost("{id:int}/devolver")]
    public async Task<ActionResult<DevolverCondicionalResponse>> Devolver([FromRoute] int id)
    {
        try
        {
            var resultado = await _mediator.Send(new DevolverCondicionalRequest { Id = id });
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }
}
