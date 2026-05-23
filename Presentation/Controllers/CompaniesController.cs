using CRMVEXA.Application.Features.Personalizacao;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.Presentation.Controllers;

/// <summary>
/// Configurações da empresa do usuário autenticado (tema, logo, módulos ativos).
/// </summary>
[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retorna personalização da empresa logada para renderização do front.
    /// </summary>
    [HttpGet("me/settings")]
    [ProducesResponseType(typeof(EmpresaSettingsMeResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EmpresaSettingsMeResponse>> MinhasSettings() =>
        Ok(await _mediator.Send(new GetMinhaEmpresaSettingsRequest()));
}
