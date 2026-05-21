using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class BazarSacolaConfiguration : IEntityTypeConfiguration<BazarSacola>
{
    public void Configure(EntityTypeBuilder<BazarSacola> builder)
    {
        builder.ToTable("BazarSacolas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataCadastro).HasColumnType("date");

        builder.HasOne(x => x.Consignante)
            .WithMany(c => c.Sacolas)
            .HasForeignKey(x => x.ConsignanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.EmpresaId, x.ConsignanteId, x.DataCadastro });
    }
}
