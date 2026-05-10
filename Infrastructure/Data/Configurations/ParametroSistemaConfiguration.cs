using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class ParametroSistemaConfiguration : IEntityTypeConfiguration<ParametroSistema>
{
    public void Configure(EntityTypeBuilder<ParametroSistema> builder)
    {
        builder.ToTable("ParametrosSistema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Chave).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Valor).IsRequired();
        builder.Property(x => x.Tipo).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Rotulo).HasMaxLength(150);
        builder.Property(x => x.Descricao).HasMaxLength(2000);
        builder.HasIndex(x => new { x.EmpresaId, x.Chave }).IsUnique();
        builder.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.AtualizadoPorUsuario).WithMany().HasForeignKey(x => x.AtualizadoPorUsuarioId).OnDelete(DeleteBehavior.SetNull);
    }
}
