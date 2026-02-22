using CRMVEXA.Application.Features.Usuarios.GetListaUsuarios;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Usuarios.GetListaUsuarios
{
    public class GetListaUsuariosHandler : IRequestHandler<GetListaUsuariosRequest, GetListaUsuariosResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetListaUsuariosHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<GetListaUsuariosResponse> Handle(GetListaUsuariosRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var query = _context.Usuarios.Where(u => u.EmpresaId == empresaId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Valor))
            {
                var valor = request.Valor.ToLower();
                if (request.Filtro?.ToLower() == "email")
                {
                    query = query.Where(u => u.Email.ToLower().Contains(valor));
                }
                else if (request.Filtro?.ToLower() == "login")
                {
                    query = query.Where(u => u.Login.ToLower().Contains(valor));
                }
                else if (request.Filtro?.ToLower() == "cargo")
                {
                    query = query.Where(u => u.Cargo.ToLower().Contains(valor));
                }
                else
                {
                    query = query.Where(u => u.Nome.ToLower().Contains(valor));
                }
            }

            var pagina = Math.Max(1, request.Pagina);
            var tamanho = Math.Clamp(request.Tamanho, 1, 50);

            // Total de registros (para o front saber que há mais páginas)
            var totalRegistros = await query.CountAsync(cancellationToken);
            var totalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling(totalRegistros / (double)tamanho)
                : 0;

            var usuarios = await query
                .Include(u => u.Enderecos)
                .Include(u => u.UsuarioPermissoes)
                .OrderBy(u => u.Nome)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync(cancellationToken);

            var usuariosResponse = usuarios.Select(u => new UsuarioResponse
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Login = u.Login,
                Cargo = u.Cargo,
                CPF = u.CPF,
                CNPJ = u.CNPJ,
                RG = u.RG,
                Telefone = u.Telefone,
                WhatsApp = u.WhatsApp,
                DataNascimento = u.DataNascimento,
                Perfil = u.Perfil,
                Departamento = u.Departamento,
                Observacoes = u.Observacoes,
                Ativo = u.Ativo,
                PodeVender = u.PodeVender,
                PodeEditarEstoque = u.PodeEditarEstoque,
                PodeEditarProduto = u.PodeEditarProduto,
                PodeAbrirFecharCaixa = u.PodeAbrirFecharCaixa,
                PodeEmitirRelatorios = u.PodeEmitirRelatorios,
                PodeCadastrarFornecedores = u.PodeCadastrarFornecedores,
                Enderecos = u.Enderecos.Select(e => new EnderecoResponse
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
                PermissaoIds = u.UsuarioPermissoes.Select(up => up.PermissaoId).ToList()
            }).ToList();

            return new GetListaUsuariosResponse
            {
                Usuarios = usuariosResponse,
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                TamanhoPagina = tamanho
            };
        }
    }
}
