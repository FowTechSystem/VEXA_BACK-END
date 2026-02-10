using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
    {
        public void Configure(EntityTypeBuilder<Estoque> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.LocalizacaoEstoque).HasMaxLength(100);
            builder.HasIndex(x => x.ProdutoId).IsUnique(); // 1:1 com Produto
            builder.ToTable("Estoques");
        }
    }
}
