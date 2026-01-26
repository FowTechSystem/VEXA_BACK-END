using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.SKU)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(x => x.Categoria)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(x => x.Genero)
                .IsRequired()
                .HasMaxLength(20);
            
            builder.Property(x => x.PrecoCusto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            builder.Property(x => x.PrecoVenda)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Peso)
                .HasColumnType("decimal(18,2)");

            builder.HasOne<Fornecedor>()
                .WithMany()
                .HasForeignKey(x => x.FornecedorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.ToTable("Produtos");
        }
    }
}
