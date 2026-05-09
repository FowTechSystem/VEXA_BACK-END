using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class PerfilAcessoConfiguration : IEntityTypeConfiguration<PerfilAcesso>
{
    public void Configure(EntityTypeBuilder<PerfilAcesso> builder)
    {
        builder.ToTable("PerfisAcesso");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nome).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Rotulo).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Descricao).HasMaxLength(2000);
        builder.HasIndex(x => new { x.EmpresaId, x.Nome }).IsUnique();
        builder.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
    }
}
