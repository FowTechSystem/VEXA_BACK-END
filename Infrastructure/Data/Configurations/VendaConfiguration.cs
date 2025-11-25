using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class VendaConfiguration : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CodigoVenda).IsRequired().HasMaxLength(50);
            builder.Property(x => x.FormaPagamento).HasMaxLength(30);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(20);
            builder.HasMany(x => x.Itens).WithOne().HasForeignKey("VendaId");
        }
    }
}
