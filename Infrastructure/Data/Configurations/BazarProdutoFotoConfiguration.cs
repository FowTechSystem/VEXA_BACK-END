using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class BazarProdutoFotoConfiguration : IEntityTypeConfiguration<BazarProdutoFoto>
{
    public void Configure(EntityTypeBuilder<BazarProdutoFoto> builder)
    {
        builder.ToTable("BazarProdutoFotos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url).IsRequired().HasMaxLength(500);

        builder.HasOne(x => x.Produto)
            .WithMany(p => p.Fotos)
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ProdutoId, x.Ordem });
        builder.ToTable(t => t.HasCheckConstraint("CK_BazarProdutoFotos_Ordem", "\"Ordem\" >= 0 AND \"Ordem\" <= 2"));
    }
}
