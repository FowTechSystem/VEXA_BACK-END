using CRMVEXA.Application.Features.Clientes.CriarCliente;
using CRMVEXA.Application.Features.Clientes.DeleteCliente;
using CRMVEXA.Application.Features.Clientes.GetCliente;
using CRMVEXA.Application.Features.Clientes.GetListaClientes;
using CRMVEXA.Application.Features.Clientes.UpdateCliente;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;

        public ClientesController(IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> ObterPorId([FromRoute] int id)
        {
            try
            {
                var resultado = await _mediator.Send(new GetClienteRequest { Id = id });
                if (resultado == null)
                    return NotFound(new { sucesso = false, mensagem = "Cliente não encontrado." });
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<GetListaClientesResponse>> Listar(
            [FromQuery] string? filtro,
            [FromQuery] string? valor,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 100)
        {
            try
            {
                var request = new GetListaClientesRequest
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
        public async Task<ActionResult<CriarClienteResponse>> Criar([FromBody] CriarClienteRequest request)
        {
            try
            {
                var cliente = await _mediator.Send(request);
                return StatusCode(201, cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateClienteResponse>> Atualizar(
            [FromRoute] int id,
            [FromBody] UpdateClienteRequest request)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar([FromRoute] int id)
        {
            try
            {
                var sucesso = await _mediator.Send(new DeleteClienteRequest { id = id });

                if (!Convert.ToBoolean(sucesso))
                    return NotFound(new { sucesso = false, mensagem = "Cliente não encontrado." });

                return Ok(new { sucesso = true, mensagem = "Cliente deletado!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

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
                  
                var nomeArquivo = $"{Guid.NewGuid()}{extensao}";

                var pastaUploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "clientes");

                if (!Directory.Exists(pastaUploads))
                    Directory.CreateDirectory(pastaUploads);

                var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var url = $"/uploads/clientes/{nomeArquivo}";

                return Ok(new { sucesso = true, url, fotoUrl = url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}
