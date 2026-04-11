using CRMVEXA.Application.Features.Clientes;
using CRMVEXA.Application.Features.Usuarios.GetListaUsuarios;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Usuarios.GetUsuario
{
    public class GetUsuarioHandler : IRequestHandler<GetUsuarioRequest, UsuarioResponse?>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetUsuarioHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<UsuarioResponse?> Handle(GetUsuarioRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var usuario = await _context.Usuarios
                .Where(u => u.EmpresaId == empresaId)
                .Include(u => u.Enderecos)
                .Include(u => u.UsuarioPermissoes)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (usuario == null)
                return null;

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Login = usuario.Login,
                Cargo = usuario.Cargo,
                CPF = usuario.CPF,
                CNPJ = usuario.CNPJ,
                RG = usuario.RG,
                Telefone = usuario.Telefone,
                WhatsApp = usuario.WhatsApp,
                DataNascimento = usuario.DataNascimento,
                Perfil = usuario.Perfil,
                Departamento = usuario.Departamento,
                Observacoes = usuario.Observacoes,
                Ativo = usuario.Ativo,
                PodeVender = usuario.PodeVender,
                PodeEditarEstoque = usuario.PodeEditarEstoque,
                PodeEditarProduto = usuario.PodeEditarProduto,
                PodeAbrirFecharCaixa = usuario.PodeAbrirFecharCaixa,
                PodeEmitirRelatorios = usuario.PodeEmitirRelatorios,
                PodeCadastrarFornecedores = usuario.PodeCadastrarFornecedores,
                FotoUrl = ClienteFotoUrlHelper.ToApiFotoUrl(usuario.Foto),
                Enderecos = usuario.Enderecos.Select(e => new EnderecoResponse
                {
                    Id = e.Id,
                    Cep = e.Cep,
                    Rua = e.Rua,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    Bairro = e.Bairro,
                    Cidade = e.Cidade,
                    Estado = e.Estado
                }).ToList(),
                PermissaoIds = usuario.UsuarioPermissoes.Select(up => up.PermissaoId).ToList()
            };
        }
    }
}
