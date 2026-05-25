using CRMVEXA.Application.Constants;
using CRMVEXA.Application.Features.Personalizacao;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.Presentation.Controllers;

/// <summary>
/// Personalização white-label por empresa (apenas perfil Dev).
/// </summary>
[ApiController]
[Route("api/dev/companies")]
[Authorize]
public class DevCompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DevCompaniesController(IMediator mediator) => _mediator = mediator;

    private bool EhDev() => SistemaConstants.IsUsuarioDevAdmin(User);

    private IActionResult AcessoNegado() => StatusCode(403, new
    {
        error = "FORBIDDEN",
        message = "Acesso restrito ao administrador de desenvolvimento."
    });

    [HttpGet("{companyId:int}/settings")]
    [ProducesResponseType(typeof(EmpresaSettingsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterSettings(int companyId)
    {
        if (!EhDev()) return AcessoNegado();
        return Ok(await _mediator.Send(new GetDevEmpresaSettingsRequest(companyId)));
    }

    [HttpPut("{companyId:int}/settings")]
    public async Task<IActionResult> AtualizarSettings(int companyId, [FromBody] AtualizarEmpresaSettingsRequest body)
    {
        if (!EhDev()) return AcessoNegado();
        try
        {
            return Ok(await _mediator.Send(new UpdateDevEmpresaSettingsRequest(companyId, body)));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{companyId:int}/logo")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> UploadLogo(int companyId, IFormFile file)
    {
        if (!EhDev()) return AcessoNegado();
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Envie um arquivo de logo." });
        try
        {
            return Ok(await _mediator.Send(new UploadDevEmpresaLogoRequest(companyId, file)));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{companyId:int}/logo")]
    public async Task<IActionResult> RemoverLogo(int companyId)
    {
        if (!EhDev()) return AcessoNegado();
        await _mediator.Send(new DeleteDevEmpresaLogoRequest(companyId));
        return NoContent();
    }

    [HttpPut("{companyId:int}/modules")]
    public async Task<IActionResult> AtualizarModulos(int companyId, [FromBody] AtualizarEmpresaModulosRequest body)
    {
        if (!EhDev()) return AcessoNegado();
        try
        {
            return Ok(await _mediator.Send(new UpdateDevEmpresaModulosRequest(companyId, body)));
        }
        catch (ModuloValidacaoException ex)
        {
            return BadRequest(new
            {
                error = ex.ErrorCode,
                message = ex.Message,
                blocking_modules = ex.BlockingModules
            });
        }
    }
}
