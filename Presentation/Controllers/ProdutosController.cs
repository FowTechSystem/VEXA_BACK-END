using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Features.Produtos.CreateProduto;
using CRMVEXA.Application.Features.Produtos.UpdateProduto;
using CRMVEXA.Application.Features.Produtos.GetProduto;
using CRMVEXA.Application.Features.Produtos.GetListaProdutos;
using CRMVEXA.Application.Features.Produtos.DeleteProduto;

namespace CRMVEXA.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProdutosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<GetListaProdutosResponse>> Listar(
            [FromQuery] string? filtro,
            [FromQuery] string? valor,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 100)
        {
            try
            {
                var request = new GetListaProdutosRequest
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
                return BadRequest(Erro(ObterMensagemErro(ex)));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProdutoResponse>> ObterPorId(int id)
        {
            try
            {
                var resultado = await _mediator.Send(new GetProdutoRequest { Id = id });
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(Erro(ObterMensagemErro(ex)));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CreateProdutoRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(Erro("Corpo da requisição inválido."));

                var id = await _mediator.Send(request);
                return Ok(new { sucesso = true, id, mensagem = "Produto criado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(Erro(ObterMensagemErro(ex)));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(
            [FromRoute] int id,
            [FromBody] UpdateProdutoRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(Erro("Corpo da requisição inválido."));

                request.Id = id;
                var resultado = await _mediator.Send(request);
                return Ok(new { sucesso = true, id = resultado.Id, mensagem = resultado.Mensagem });
            }
            catch (Exception ex)
            {
                return BadRequest(Erro(ObterMensagemErro(ex)));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar([FromRoute] int id)
        {
            try
            {
                await _mediator.Send(new DeleteProdutoRequest { Id = id });
                return Ok(new { sucesso = true, mensagem = "Produto deletado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(Erro(ObterMensagemErro(ex)));
            }
        }

        /// <summary>Multipart → data URL para <c>foto1Url</c>…<c>foto5Url</c> no PUT (igual cliente/usuário). Sem gravar em disco.</summary>
        [HttpPost("upload-foto")]
        public async Task<ActionResult> UploadFoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(Erro("Nenhum arquivo enviado."));

            try
            {
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extensao = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensao))
                    return BadRequest(Erro("Formato de arquivo não permitido. Use JPG, PNG, GIF ou WEBP."));

                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(Erro("Arquivo muito grande. Tamanho máximo: 5MB."));

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
                return BadRequest(Erro(ObterMensagemErro(ex)));
            }
        }

        /// <summary>Obtém a mensagem mais relevante (inner exception para erros de DB).</summary>
        private static string ObterMensagemErro(Exception ex)
        {
            var atual = ex;
            while (atual?.InnerException != null)
                atual = atual.InnerException;
            return atual?.Message ?? ex.Message;
        }

        /// <summary>Resposta de erro padronizada para o frontend (sucesso, mensagem e message).</summary>
        private static object Erro(string mensagem) => new
        {
            sucesso = false,
            mensagem,
            message = mensagem
        };
    }
}
