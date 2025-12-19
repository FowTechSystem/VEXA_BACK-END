using CRMVEXA.Application.Features.Clientes.CriarCliente;
using CRMVEXA.Application.Features.Clientes.DeleteCliente;
using CRMVEXA.Application.Features.Clientes.GetListaClientes;
using CRMVEXA.Application.Features.Clientes.UpdateCliente;
using CRMVEXA.Application.Features.Fornecedor.CreateFornecedor;
using CRMVEXA.Application.Features.Fornecedor.DeleteFornecedor;
using CRMVEXA.Application.Features.Fornecedor.GetFornecedor;
using CRMVEXA.Application.Features.Fornecedor.UpdateClienteFornecedor;
using CRMVEXA.Application.Features.Fornecedor.UpdateFornecedor;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CRMVEXA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FornecedoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public FornecedoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetFornecedorResponse>> Listar(  [FromQuery] string? filtro, [FromQuery] string? valor,
           [FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
    {
        try
        {
            var request = new GetFornecedorRequest
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
    public async Task<ActionResult<int>> Create([FromBody] CreateFornecedorRequest request)
    {
        try
        {
            var id = await _mediator.Send(request);
            return Ok(new 
            {
                sucesso = true,
                id,
                mensagem = "Fornecedor criado com sucesso!" 
            }
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateFornecedorResponse>> Update([FromRoute] int id, [FromBody] UpdateFornecedorRequest request)
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
            return BadRequest(new 
            { 
                sucesso = false,
                mensagem = ex.Message 
            }
            );
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var sucesso = await _mediator.Send(new DeleteFornecedorRequest { id = id });

            if (!Convert.ToBoolean(sucesso))
                return NotFound(new 
                {
                    sucesso = false,
                    mensagem = "Fornecedor não encontrado." 
                });

            return Ok(new 
            { 
                sucesso = true,
                mensagem = "Fornecedor deletado!" 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new 
            {
                sucesso = false,
                mensagem = ex.Message 
            });
        }
    }

}
