using CRMVEXA.Application.Features.Auth.ForgotPassword;
using CRMVEXA.Application.Features.Auth.Login;
using CRMVEXA.Application.Features.Auth.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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

        /// <summary>Solicita link de redefinição de senha (resposta genérica; não revela se o e-mail existe).</summary>
        [HttpPost("forgot-password")]
        [EnableRateLimiting("auth-password-recovery")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Informe um e-mail válido." });

            try
            {
                var res = await _mediator.Send(request);
                return Ok(new { message = res.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro em forgot-password para {Email}", request.Email);
                var msg = _env.IsDevelopment() ? ex.Message : "Erro ao processar solicitação.";
                return StatusCode(500, new { message = msg });
            }
        }

        /// <summary>Redefine a senha com o token recebido por e-mail (query da rota do front).</summary>
        [HttpPost("reset-password")]
        [EnableRateLimiting("auth-password-recovery")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest? request)
        {
            if (request == null)
                return BadRequest(new { message = "Corpo da requisição inválido." });

            try
            {
                var result = await _mediator.Send(request);
                if (!result.Success)
                    return BadRequest(new { message = result.ErrorMessage, Message = result.ErrorMessage });
                return Ok(new { message = "Senha alterada com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro em reset-password");
                var msg = _env.IsDevelopment() ? ex.Message : "Erro ao redefinir senha.";
                return StatusCode(500, new { message = msg });
            }
        }
    }
}
