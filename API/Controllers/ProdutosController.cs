using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Features.Produtos.CreateProduto;
using CRMVEXA.Application.Features.Produtos.UpdateProduto;
using CRMVEXA.Application.Features.Produtos.GetProduto;
using CRMVEXA.Application.Features.Produtos.GetListaProdutos;
using CRMVEXA.Application.Features.Produtos.DeleteProduto;

namespace CRMVEXA.API.Controllers
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
            [FromQuery] int tamanho = 10)
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
                return BadRequest(Erro(ex.Message));
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
                return BadRequest(Erro(ex.Message));
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
                return BadRequest(Erro(ex.Message));
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
                return BadRequest(Erro(ex.Message));
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
                return BadRequest(Erro(ex.Message));
            }
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
