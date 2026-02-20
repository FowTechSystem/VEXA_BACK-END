using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRMVEXA.Application.Features.Auth.Login
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse?>
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginHandler(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);

            if (usuario == null)
                return null;

            var senhaHash = usuario.SenhaHash;
            if (string.IsNullOrEmpty(senhaHash))
                return null;

            // Suporta senha em texto (legado) ou hash BCrypt
            bool senhaValida = false;
            if (senhaHash.StartsWith("$2") && senhaHash.Length > 50)
            {
                try
                {
                    senhaValida = BCrypt.Net.BCrypt.Verify(request.Password, senhaHash);
                }
                catch
                {
                    senhaValida = false;
                }
            }
            else
            {
                senhaValida = senhaHash == request.Password;
            }

            if (!senhaValida)
                return null;

            var token = GerarJwt(usuario);
            return new LoginResponse
            {
                Token = token,
                User = new LoginUserResponse
                {
                    Id = usuario.Id,
                    EmpresaId = usuario.EmpresaId,
                    EmpresaNome = usuario.Empresa?.Nome ?? string.Empty,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Perfil = usuario.Cargo ?? "Usuário"
                }
            };
        }

        private string GerarJwt(Usuario usuario)
        {
            var secret = _configuration["Jwt:Secret"] ?? "ChaveSecretaMinima32Caracteres!!";
            var issuer = _configuration["Jwt:Issuer"] ?? "CRMVEXA";
            var audience = _configuration["Jwt:Audience"] ?? "CRMVEXA";
            var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var min) ? min : 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Email, usuario.Email ?? ""),
                new(ClaimTypes.Name, usuario.Nome ?? ""),
                new("cargo", usuario.Cargo ?? ""),
                new("empresa_id", usuario.EmpresaId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
