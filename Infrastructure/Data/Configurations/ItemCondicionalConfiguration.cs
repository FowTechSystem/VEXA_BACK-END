using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class ItemCondicionalConfiguration : IEntityTypeConfiguration<ItemCondicional>
{
    public void Configure(EntityTypeBuilder<ItemCondicional> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantidade).IsRequired();
        builder.Property(x => x.PrecoUnitario).HasColumnType("decimal(18,2)").IsRequired();

        builder.HasOne<Produto>()
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("ItensCondicional");
    }
}
