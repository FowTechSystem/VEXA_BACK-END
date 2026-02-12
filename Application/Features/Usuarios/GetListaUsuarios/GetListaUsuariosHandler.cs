using CRMVEXA.Application.Features.Usuarios.GetListaUsuarios;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Usuarios.GetListaUsuarios
{
    public class GetListaUsuariosHandler : IRequestHandler<GetListaUsuariosRequest, GetListaUsuariosResponse>
    {
        private readonly AppDbContext _context;

        public GetListaUsuariosHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetListaUsuariosResponse> Handle(GetListaUsuariosRequest request, CancellationToken cancellationToken)
        {
            var query = _context.Usuarios.AsQueryable();

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
                PodeVender = u.PodeVender,
                PodeEditarEstoque = u.PodeEditarEstoque,
                PodeEditarProduto = u.PodeEditarProduto,
                PodeAbrirFecharCaixa = u.PodeAbrirFecharCaixa,
                PodeEmitirRelatorios = u.PodeEmitirRelatorios,
                PodeCadastrarFornecedores = u.PodeCadastrarFornecedores
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
