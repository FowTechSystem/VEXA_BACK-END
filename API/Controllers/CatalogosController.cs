using CRMVEXA.Application.Features.Catalogo.CreateCatalogo;
using CRMVEXA.Application.Features.Catalogo.DeleteCatalogo;
using CRMVEXA.Application.Features.Catalogo.GetCatalogoPublico;
using CRMVEXA.Application.Features.Catalogo.GetCatalogoPublicoPorSlug;
using CRMVEXA.Application.Features.Catalogo.GetListaCatalogos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CatalogosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateCatalogoResponse>> Criar([FromBody] CreateCatalogoRequest? request)
    {
        if (request == null)
            return BadRequest(new { sucesso = false, mensagem = "Corpo inválido. Envie produtos, criadorWhatsApp, criadorNome, empresaNome, slug (opcional)." });

        try
        {
            var resultado = await _mediator.Send(request);
            return StatusCode(201, resultado);
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

    [AllowAnonymous]
    [HttpGet("publico/{id:int}")]
    public async Task<ActionResult> ObterPublicoPorId([FromRoute] int id)
    {
        var resultado = await _mediator.Send(new GetCatalogoPublicoRequest { Id = id });
        if (resultado == null)
            return NotFound(new { sucesso = false, mensagem = "Catálogo não encontrado ou inativo." });
        return Ok(resultado);
    }

    [AllowAnonymous]
    [HttpGet("publico/slug/{slug}")]
    public async Task<ActionResult> ObterPublicoPorSlug([FromRoute] string slug)
    {
        var resultado = await _mediator.Send(new GetCatalogoPublicoPorSlugRequest { Slug = slug });
        if (resultado == null)
            return NotFound(new { sucesso = false, mensagem = "Catálogo não encontrado ou inativo." });
        return Ok(resultado);
    }

    [HttpGet]
    public async Task<ActionResult<GetListaCatalogosResponse>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 20)
    {
        try
        {
            var resultado = await _mediator.Send(new GetListaCatalogosRequest { Pagina = pagina, Tamanho = tamanho });
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Excluir([FromRoute] int id)
    {
        var sucesso = await _mediator.Send(new DeleteCatalogoRequest { Id = id });
        if (!sucesso)
            return NotFound(new { sucesso = false, mensagem = "Catálogo não encontrado." });
        return NoContent();
    }
}
