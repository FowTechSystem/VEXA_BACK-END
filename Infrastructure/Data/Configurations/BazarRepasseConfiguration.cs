using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class BazarRepasseConfiguration : IEntityTypeConfiguration<BazarRepasse>
{
    public void Configure(EntityTypeBuilder<BazarRepasse> builder)
    {
        builder.ToTable("BazarRepasses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ValorTotal).HasColumnType("decimal(10,2)");
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(BazarRepasseStatus.Pendente);
        builder.Property(x => x.FormaPagamento).HasMaxLength(30);

        builder.HasOne(x => x.Consignante)
            .WithMany(c => c.Repasses)
            .HasForeignKey(x => x.ConsignanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CriadoPor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PagoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.PagoPor)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.EmpresaId, x.Status, x.DataGeracao });
        builder.HasIndex(x => new { x.EmpresaId, x.ConsignanteId, x.Status });
    }
}
