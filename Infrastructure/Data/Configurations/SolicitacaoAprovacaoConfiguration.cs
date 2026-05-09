using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class SolicitacaoAprovacaoConfiguration : IEntityTypeConfiguration<SolicitacaoAprovacao>
{
    public void Configure(EntityTypeBuilder<SolicitacaoAprovacao> builder)
    {
        builder.ToTable("SolicitacoesAprovacao");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PermissaoChave).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ContextoJson).HasColumnType("jsonb");
        builder.Property(x => x.Estado).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Motivo).HasMaxLength(2000);
        builder.Property(x => x.RespostaNota).HasMaxLength(2000);
        builder.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SolicitadoPorUsuario).WithMany().HasForeignKey(x => x.SolicitadoPorUsuarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AprovadoPorUsuario).WithMany().HasForeignKey(x => x.AprovadoPorUsuarioId).OnDelete(DeleteBehavior.SetNull);
    }
}
