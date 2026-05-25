using System.Security.Claims;
using System.Text.Json;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace CRMVEXA.Application.Services.Bazar;

public class BazarAuditoriaService
{
    private readonly AppDbContext _db;
    private readonly IEmpresaContext _empresa;
    private readonly IHttpContextAccessor _http;

    public BazarAuditoriaService(AppDbContext db, IEmpresaContext empresa, IHttpContextAccessor http)
    {
        _db = db;
        _empresa = empresa;
        _http = http;
    }

    public void RegistrarExclusao(string entidade, int recursoId, object? detalhes = null)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId <= 0)
            return;

        _db.AuditoriaAcoes.Add(new AuditoriaAcao
        {
            EmpresaId = _empresa.EmpresaId,
            UsuarioId = usuarioId,
            Acao = $"bazar.excluir.{entidade}",
            RecursoId = recursoId.ToString(),
            DepoisJson = JsonSerializer.Serialize(new
            {
                entidade,
                recursoId,
                detalhes,
                timestamp = DateTime.UtcNow
            }),
            EnderecoIp = _http.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            CriadoEm = DateTime.UtcNow
        });
    }

    private int ObterUsuarioId()
    {
        var idStr = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}
