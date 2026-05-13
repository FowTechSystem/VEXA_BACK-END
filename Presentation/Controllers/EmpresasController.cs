using CRMVEXA.Application.Constants;
using CRMVEXA.Application.Features.Empresas.CriarEmpresa;
using CRMVEXA.Application.Features.Empresas.CriarPrimeiroUsuario;
using CRMVEXA.Application.Features.Empresas.GetListaEmpresas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.Presentation.Controllers
{
    /// <summary>
    /// Cadastro e listagem de empresas. Apenas o usuário dev@gmail.com (administrador do sistema) pode acessar.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpresasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmpresasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private bool UsuarioPodeGerenciarEmpresas() => SistemaConstants.IsUsuarioDevAdmin(User);

        private static object RespostaAcessoNegadoDev() => new
        {
            message = "Acesso restrito ao administrador de desenvolvimento."
        };

        /// <summary>
        /// Lista todas as empresas. Apenas dev@gmail.com.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<GetListaEmpresasResponse>> Listar()
        {
            if (!UsuarioPodeGerenciarEmpresas())
                return StatusCode(403, RespostaAcessoNegadoDev());

            try
            {
                var resultado = await _mediator.Send(new GetListaEmpresasRequest());
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova empresa. Apenas dev@gmail.com.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CriarEmpresaResponse>> Criar([FromBody] CriarEmpresaRequest request)
        {
            if (!UsuarioPodeGerenciarEmpresas())
                return StatusCode(403, RespostaAcessoNegadoDev());

            if (string.IsNullOrWhiteSpace(request?.Nome))
            {
                return BadRequest(new
                {
                    message = "Nome da empresa é obrigatório.",
                    errors = new { Nome = new[] { "O campo Nome é obrigatório." } }
                });
            }

            try
            {
                var resultado = await _mediator.Send(request);
                return StatusCode(201, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cria o primeiro usuário de uma empresa (para o cliente acessar o sistema). Apenas dev@gmail.com.
        /// Body: { nome, email, senha }
        /// </summary>
        [HttpPost("{id:int}/primeiro-usuario")]
        public async Task<ActionResult<CriarPrimeiroUsuarioResponse>> CriarPrimeiroUsuario(
            [FromRoute] int id,
            [FromBody] CriarPrimeiroUsuarioBodyRequest body)
        {
            if (!UsuarioPodeGerenciarEmpresas())
                return StatusCode(403, RespostaAcessoNegadoDev());

            if (string.IsNullOrWhiteSpace(body?.Nome) || string.IsNullOrWhiteSpace(body?.Email) || string.IsNullOrWhiteSpace(body?.Senha))
            {
                return BadRequest(new { message = "Nome, e-mail e senha são obrigatórios." });
            }

            var request = new CriarPrimeiroUsuarioRequest
            {
                EmpresaId = id,
                Nome = body.Nome,
                Email = body.Email,
                Senha = body.Senha
            };

            var resultado = await _mediator.Send(request);
            if (!resultado.Sucesso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }

    public class CriarPrimeiroUsuarioBodyRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
