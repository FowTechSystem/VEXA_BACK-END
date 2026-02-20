using CRMVEXA.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        private readonly IWebHostEnvironment _env;

        public AuthController(IMediator mediator, ILogger<AuthController> logger, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Autentica usuário por e-mail e senha. Retorna JWT e dados do usuário.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos" });
            }

            try
            {
                var result = await _mediator.Send(request);

                if (result == null)
                {
                    return Unauthorized(new { message = "E-mail ou senha inválidos" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar login para email: {Email}", request?.Email ?? "(null)");
                var msg = _env.IsDevelopment() ? ex.Message : "Erro interno do servidor";
                return StatusCode(500, new { message = msg, detail = _env.IsDevelopment() ? ex.ToString() : null });
            }
        }
    }
}
