using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class AuditoriaAcaoConfiguration : IEntityTypeConfiguration<AuditoriaAcao>
{
    public void Configure(EntityTypeBuilder<AuditoriaAcao> builder)
    {
        builder.ToTable("AuditoriaAcoes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Acao).IsRequired().HasMaxLength(100);
        builder.Property(x => x.RecursoId).HasMaxLength(64);
        builder.Property(x => x.AntesJson).HasColumnType("jsonb");
        builder.Property(x => x.DepoisJson).HasColumnType("jsonb");
        builder.Property(x => x.EnderecoIp).HasMaxLength(45);
        builder.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.CriadoEm);
        builder.HasIndex(x => new { x.EmpresaId, x.Acao });
    }
}
