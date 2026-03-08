using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class CatalogoConfiguration : IEntityTypeConfiguration<Catalogo>
{
    public void Configure(EntityTypeBuilder<Catalogo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CriadorWhatsApp).IsRequired().HasMaxLength(20);
        builder.Property(x => x.CriadorNome).HasMaxLength(200);
        builder.Property(x => x.EmpresaNome).HasMaxLength(200);
        builder.Property(x => x.Slug).HasMaxLength(100);
        builder.Property(x => x.CriadoEm).IsRequired();
        builder.Property(x => x.Ativo).IsRequired().HasDefaultValue(true);

        builder.HasMany(c => c.Produtos)
            .WithOne()
            .HasForeignKey("CatalogoId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Criador)
            .WithMany()
            .HasForeignKey(c => c.CriadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Slug);
        builder.HasIndex(x => new { x.EmpresaId, x.Slug });

        builder.ToTable("Catalogos");
    }
}
