using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
    {
        public void Configure(EntityTypeBuilder<ItemVenda> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantidade)
                .IsRequired();

            builder.Property(x => x.PrecoUnitario)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TipoItem)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("normal");

            builder.HasOne<Produto>()
                .WithMany()
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<BazarProduto>()
                .WithMany()
                .HasForeignKey(x => x.BazarProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("ItensVenda");
        }
    }
}
