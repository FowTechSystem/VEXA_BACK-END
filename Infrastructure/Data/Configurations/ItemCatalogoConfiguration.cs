using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class ItemCatalogoConfiguration : IEntityTypeConfiguration<ItemCatalogo>
{
    public void Configure(EntityTypeBuilder<ItemCatalogo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nome).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Preco).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Quantidade).IsRequired().HasDefaultValue(1);

        builder.HasOne<Produto>()
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("ItensCatalogo");
    }
}
