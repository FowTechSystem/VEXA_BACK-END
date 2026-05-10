using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class PermissaoCatalogoConfiguration : IEntityTypeConfiguration<PermissaoCatalogo>
{
    public void Configure(EntityTypeBuilder<PermissaoCatalogo> builder)
    {
        builder.ToTable("PermissoesCatalogo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Modulo).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Acao).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Recurso).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Chave).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Rotulo).IsRequired().HasMaxLength(150);
        builder.HasIndex(x => x.Chave).IsUnique();
        builder.HasIndex(x => new { x.Modulo, x.Acao, x.Recurso }).IsUnique();
    }
}
