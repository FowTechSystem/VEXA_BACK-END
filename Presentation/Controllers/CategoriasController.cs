using CRMVEXA.Application.Features.Categorias.CreateCategoria;
using CRMVEXA.Application.Features.Categorias.DeleteCategoria;
using CRMVEXA.Application.Features.Categorias.GetCategoria;
using CRMVEXA.Application.Features.Categorias.GetListaCategorias;
using CRMVEXA.Application.Features.Categorias.UpdateCategoria;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetListaCategoriasResponse>> Listar(CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _mediator.Send(new GetListaCategoriasRequest(), cancellationToken);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetCategoriaResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _mediator.Send(new GetCategoriaRequest { Id = id }, cancellationToken);
            if (resultado == null)
                return NotFound(new { sucesso = false, mensagem = "Categoria não encontrada.", message = "Categoria não encontrada." });
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateCategoriaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Nome))
                return BadRequest(new { sucesso = false, mensagem = "Nome é obrigatório.", message = "Nome é obrigatório." });
            var id = await _mediator.Send(request, cancellationToken);
            return Ok(new { sucesso = true, id, mensagem = "Categoria criada com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message, message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateCategoriaRequest request, CancellationToken cancellationToken)
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
            var resultado = await _mediator.Send(new DeleteCategoriaRequest { Id = id }, cancellationToken);
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
