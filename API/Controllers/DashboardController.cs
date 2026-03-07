using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Features.Dashboard.GetDashboardData;

namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retorna todas as métricas do Dashboard para o mês/ano selecionado.
        /// Requer autenticação e EmpresaId (token JWT ou header X-Empresa-Id para dev).
        /// </summary>
        /// <param name="mes">Mês (1-12). Se não informado, usa o mês atual.</param>
        /// <param name="ano">Ano. Se não informado, usa o ano atual.</param>
        [HttpGet]
        public async Task<ActionResult<GetDashboardDataResponse>> Obter(
            [FromQuery] int? mes = null,
            [FromQuery] int? ano = null)
        {
            try
            {
                var now = DateTime.UtcNow;
                var request = new GetDashboardDataRequest
                {
                    Mes = mes ?? now.Month,
                    Ano = ano ?? now.Year
                };
                var resultado = await _mediator.Send(request);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}
