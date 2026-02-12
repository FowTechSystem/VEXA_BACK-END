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
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
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
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar([FromBody] CreateProdutoRequest request)
        {
            try
            {
                var id = await _mediator.Send(request);
                return Ok(new { sucesso = true, id, mensagem = "Produto criado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateProdutoResponse>> Atualizar(
            [FromRoute] int id,
            [FromBody] UpdateProdutoRequest request)
        {
            try
            {
                request.Id = id;
                var resultado = await _mediator.Send(request);
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
                await _mediator.Send(new DeleteProdutoRequest { Id = id });
                return Ok(new { sucesso = true, mensagem = "Produto deletado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}
