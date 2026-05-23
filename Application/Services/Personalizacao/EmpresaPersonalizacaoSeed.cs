using CRMVEXA.Application.Constants;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Services.Personalizacao;

public static class EmpresaPersonalizacaoSeed
{
    public static async Task GarantirPadraoAsync(
        AppDbContext db,
        int empresaId,
        string displayName,
        int? updatedBy = null,
        CancellationToken ct = default)
    {
        var agora = DateTime.UtcNow;
        var config = await db.EmpresaConfiguracoes.FirstOrDefaultAsync(c => c.EmpresaId == empresaId, ct);
        if (config == null)
        {
            db.EmpresaConfiguracoes.Add(new EmpresaConfiguracao
            {
                EmpresaId = empresaId,
                DisplayName = displayName.Trim(),
                PrimaryColor = "#FACC15",
                SecondaryColor = "#1F2937",
                DefaultTheme = TemaPadraoEmpresa.Dark,
                UpdatedAt = agora,
                UpdatedBy = updatedBy
            });
        }

        var existentes = await db.EmpresaModulos
            .Where(m => m.EmpresaId == empresaId)
            .Select(m => m.ModuleKey)
            .ToListAsync(ct);

        foreach (var def in ModuloCatalogo.Todos)
        {
            if (existentes.Contains(def.Key, StringComparer.OrdinalIgnoreCase))
                continue;

            db.EmpresaModulos.Add(new EmpresaModulo
            {
                EmpresaId = empresaId,
                ModuleKey = def.Key,
                IsEnabled = true,
                UpdatedAt = agora,
                UpdatedBy = updatedBy
            });
        }

        await db.SaveChangesAsync(ct);
    }
}
