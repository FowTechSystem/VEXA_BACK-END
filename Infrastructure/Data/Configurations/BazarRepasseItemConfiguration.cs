using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class BazarRepasseItemConfiguration : IEntityTypeConfiguration<BazarRepasseItem>
{
    public void Configure(EntityTypeBuilder<BazarRepasseItem> builder)
    {
        builder.ToTable("BazarRepasseItens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Valor).HasColumnType("decimal(10,2)");

        builder.HasOne(x => x.Repasse)
            .WithMany(r => r.Itens)
            .HasForeignKey(x => x.RepasseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Produto)
            .WithOne(p => p.RepasseItem)
            .HasForeignKey<BazarRepasseItem>(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.RepasseId);
        builder.HasIndex(x => x.ProdutoId).IsUnique();
    }
}
