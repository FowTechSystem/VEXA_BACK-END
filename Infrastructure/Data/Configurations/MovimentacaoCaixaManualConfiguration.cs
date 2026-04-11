using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class MovimentacaoCaixaManualConfiguration : IEntityTypeConfiguration<MovimentacaoCaixaManual>
{
    public void Configure(EntityTypeBuilder<MovimentacaoCaixaManual> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TipoResumo)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.Descricao)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Categoria)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Observacoes)
            .HasMaxLength(2000);

        builder.Property(x => x.FormaPagamento)
            .HasMaxLength(30);

        builder.Property(x => x.Valor)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.DataLancamento)
            .IsRequired();

        builder.Property(x => x.CriadoEmUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.EmpresaId, x.DataLancamento });

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.ToTable("MovimentacoesCaixaManuais");
    }
}
