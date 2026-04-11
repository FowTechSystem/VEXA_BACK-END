using CRMVEXA.Application.Features.Usuarios.CriarUsuario;
using CRMVEXA.Application.Features.Usuarios.DeleteUsuario;
using CRMVEXA.Application.Features.Usuarios.GetListaUsuarios;
using CRMVEXA.Application.Features.Usuarios.GetUsuario;
using CRMVEXA.Application.Features.Usuarios.UpdateUsuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuariosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Application.Features.Usuarios.GetListaUsuarios.UsuarioResponse>> ObterPorId([FromRoute] int id)
        {
            try
            {
                var usuario = await _mediator.Send(new GetUsuarioRequest { Id = id });
                if (usuario == null)
                    return NotFound();
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<GetListaUsuariosResponse>> Listar(
            [FromQuery] string? filtro,
            [FromQuery] string? valor,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10)
        {
            try
            {
                var request = new GetListaUsuariosRequest
                {
                    Filtro = filtro,
                    Valor = valor,
                    Pagina = pagina,
                    Tamanho = tamanho
                };

                var resultado = await _mediator.Send(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar([FromBody] CriarUsuarioRequest request)
        {
            try
            {
                var id = await _mediator.Send(request);
                return Ok(new { sucesso = true, id, mensagem = "Usuário criado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateUsuarioResponse>> Atualizar(
            [FromRoute] int id,
            [FromBody] UpdateUsuarioRequest request)
        {
            try
            {
                request.Id = id;
                var resultado = await _mediator.Send(request);

                if (!resultado.Sucesso)
                    return NotFound(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        /// <summary>Multipart → data URL para o PUT em <c>fotoUrl</c>. Sem gravar em disco.</summary>
        [HttpPost("upload-foto")]
        public async Task<ActionResult> UploadFoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { sucesso = false, mensagem = "Nenhum arquivo enviado." });

            try
            {
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extensao = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensao))
                    return BadRequest(new { sucesso = false, mensagem = "Formato de arquivo não permitido. Use JPG, PNG, GIF ou WEBP." });

                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { sucesso = false, mensagem = "Arquivo muito grande. Tamanho máximo: 5MB." });

                await using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();
                var b64 = Convert.ToBase64String(bytes);
                var mime = string.IsNullOrWhiteSpace(file.ContentType) || file.ContentType == "application/octet-stream"
                    ? extensao switch
                    {
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        ".webp" => "image/webp",
                        _ => "image/jpeg"
                    }
                    : file.ContentType.Split(';')[0].Trim();
                var dataUrl = $"data:{mime};base64,{b64}";

                return Ok(new { sucesso = true, url = dataUrl, fotoUrl = dataUrl, fotoBase64 = b64 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar([FromRoute] int id)
        {
            try
            {
                var sucesso = await _mediator.Send(new DeleteUsuarioRequest { Id = id });

                if (!sucesso)
                    return NotFound(new { sucesso = false, mensagem = "Usuário não encontrado." });

                return Ok(new { sucesso = true, mensagem = "Usuário deletado!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}
