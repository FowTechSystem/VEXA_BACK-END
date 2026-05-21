using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class BazarConsignanteConfiguration : IEntityTypeConfiguration<BazarConsignante>
{
    public void Configure(EntityTypeBuilder<BazarConsignante> builder)
    {
        builder.ToTable("BazarConsignantes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Telefone).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Cpf).HasMaxLength(14);
        builder.Property(x => x.Ativo).HasDefaultValue(true);

        builder.HasIndex(x => new { x.EmpresaId, x.Ativo });
        builder.HasIndex(x => new { x.EmpresaId, x.Nome });
        builder.HasIndex(x => new { x.EmpresaId, x.Cpf })
            .IsUnique()
            .HasFilter("\"Cpf\" IS NOT NULL AND \"DeletadoEm\" IS NULL");
    }
}
