using CRMVEXA.Application.Features.Bazar;
using CRMVEXA.Application.Features.Bazar.Consignantes;
using CRMVEXA.Application.Features.Bazar.Pecas;
using CRMVEXA.Application.Features.Bazar.Repasses;
using CRMVEXA.Application.Features.Bazar.Sacolas;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.Presentation.Controllers;

[ApiController]
[Route("api/bazar")]
public class BazarController : ControllerBase
{
    private readonly IMediator _mediator;

    public BazarController(IMediator mediator) => _mediator = mediator;

    [HttpGet("consignantes")]
    public Task<ActionResult<GetListaConsignantesResponse>> ListarConsignantes(
        [FromQuery] string? busca,
        [FromQuery] bool incluirResumo = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 50) =>
        OkOrThrow(_mediator.Send(new GetListaConsignantesRequest(busca, incluirResumo, pagina, tamanho)));

    [HttpGet("consignantes/recentes")]
    public Task<ActionResult<List<BazarConsignanteResponse>>> ConsignantesRecentes([FromQuery] int limite = 6) =>
        OkOrThrow(_mediator.Send(new GetConsignantesRecentesRequest(limite)));

    [HttpGet("consignantes/resumo")]
    public Task<ActionResult<GetConsignantesResumoGeralResponse>> ResumoGeral() =>
        OkOrThrow(_mediator.Send(new GetConsignantesResumoGeralRequest()));

    [HttpGet("consignantes/{id:int}")]
    public Task<ActionResult<BazarConsignanteResponse>> ObterConsignante(int id) =>
        OkOrThrow(_mediator.Send(new GetConsignanteRequest(id)));

    [HttpGet("consignantes/{id:int}/resumo")]
    public Task<ActionResult<BazarResumoResponse>> ObterConsignanteResumo(int id) =>
        OkOrThrow(_mediator.Send(new GetConsignanteResumoRequest(id)));

    [HttpPost("consignantes")]
    public async Task<ActionResult<BazarConsignanteResponse>> CriarConsignante([FromBody] CreateConsignanteRequest request)
    {
        var result = await _mediator.Send(request);
        return CreatedAtAction(nameof(ObterConsignante), new { id = result.Id }, result);
    }

    [HttpPut("consignantes/{id:int}")]
    public Task<ActionResult<BazarConsignanteResponse>> AtualizarConsignante(int id, [FromBody] UpdateConsignanteRequest request)
    {
        request.Id = id;
        return OkOrThrow(_mediator.Send(request));
    }

    [HttpDelete("consignantes/{id:int}")]
    public async Task<IActionResult> ExcluirConsignante(int id)
    {
        await _mediator.Send(new DeleteConsignanteRequest(id));
        return NoContent();
    }

    [HttpGet("consignantes/{consignanteId:int}/sacolas")]
    public Task<ActionResult<List<BazarSacolaResponse>>> ListarSacolas(
        int consignanteId, [FromQuery] bool incluirResumo = true) =>
        OkOrThrow(_mediator.Send(new GetSacolasConsignanteRequest(consignanteId, incluirResumo)));

    [HttpGet("sacolas/{id:int}")]
    public Task<ActionResult<BazarSacolaResponse>> ObterSacola(int id) =>
        OkOrThrow(_mediator.Send(new GetSacolaRequest(id)));

    [HttpPost("sacolas")]
    public async Task<ActionResult<BazarSacolaResponse>> CriarSacola([FromBody] CreateSacolaRequest request)
    {
        var result = await _mediator.Send(request);
        return StatusCode(201, result);
    }

    [HttpPut("sacolas/{id:int}/devolver")]
    public Task<ActionResult<BazarSacolaResponse>> DevolverSacola(int id) =>
        OkOrThrow(_mediator.Send(new DevolverSacolaRequest(id)));

    [HttpGet("pecas")]
    public Task<ActionResult<GetListaPecasResponse>> ListarPecas(
        [FromQuery] int? consignanteId,
        [FromQuery] int? sacolaId,
        [FromQuery] string? status,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 50) =>
        OkOrThrow(_mediator.Send(new GetListaPecasRequest(
            consignanteId, sacolaId, status, dataInicio, dataFim, pagina, tamanho)));

    [HttpGet("pecas/{id:int}")]
    public Task<ActionResult<BazarPecaResponse>> ObterPeca(int id) =>
        OkOrThrow(_mediator.Send(new GetPecaRequest(id)));

    [HttpPut("pecas/{id:int}")]
    public Task<ActionResult<BazarPecaResponse>> AtualizarPeca(int id, [FromBody] UpdatePecaRequest request)
    {
        request.Id = id;
        return OkOrThrow(_mediator.Send(request));
    }

    [HttpPut("pecas/{id:int}/devolver")]
    public Task<ActionResult<BazarPecaResponse>> DevolverPeca(int id) =>
        OkOrThrow(_mediator.Send(new DevolverPecaRequest(id)));

    [HttpDelete("pecas/{id:int}")]
    public async Task<IActionResult> ExcluirPeca(int id)
    {
        await _mediator.Send(new DeletePecaRequest(id));
        return NoContent();
    }

    [HttpPost("pecas/{id:int}/fotos")]
    [RequestSizeLimit(30 * 1024 * 1024)]
    public Task<ActionResult<UploadPecaFotosResponse>> UploadFotos(int id, [FromForm] IFormFileCollection files) =>
        OkOrThrow(_mediator.Send(new UploadPecaFotosRequest { PecaId = id, Files = files }));

    [HttpGet("repasses/pendentes")]
    public Task<ActionResult<List<BazarRepassePendenteGrupoResponse>>> RepassesPendentes() =>
        OkOrThrow(_mediator.Send(new GetRepassesPendentesRequest()));

    [HttpGet("consignantes/{id:int}/repasses/pendentes")]
    public Task<ActionResult<List<BazarRepassePendenteGrupoResponse>>> RepassesPendentesConsignante(int id) =>
        OkOrThrow(_mediator.Send(new GetRepassesPendentesConsignanteRequest(id)));

    [HttpGet("repasses")]
    public Task<ActionResult<GetListaRepassesResponse>> ListarRepasses(
        [FromQuery] string? status,
        [FromQuery] int? consignanteId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 50) =>
        OkOrThrow(_mediator.Send(new GetListaRepassesRequest(status, consignanteId, dataInicio, dataFim, pagina, tamanho)));

    [HttpPost("repasses")]
    public async Task<ActionResult<BazarRepasseResponse>> CriarRepasse([FromBody] CreateRepasseRequest request)
    {
        var result = await _mediator.Send(request);
        return StatusCode(201, result);
    }

    [HttpPut("repasses/{id:int}/pagar")]
    public Task<ActionResult<BazarRepasseResponse>> PagarRepasse(int id, [FromBody] PagarRepasseRequest? body)
    {
        var req = body ?? new PagarRepasseRequest();
        req.Id = id;
        return OkOrThrow(_mediator.Send(req));
    }

    [HttpGet("repasses/{id:int}")]
    public Task<ActionResult<BazarRepasseResponse>> ObterRepasse(int id) =>
        OkOrThrow(_mediator.Send(new GetRepasseRequest(id)));

    [HttpDelete("repasses/{id:int}")]
    public async Task<IActionResult> CancelarRepasse(int id)
    {
        await _mediator.Send(new DeleteRepasseRequest(id));
        return NoContent();
    }

    private async Task<ActionResult<T>> OkOrThrow<T>(Task<T> task) => Ok(await task);
}
