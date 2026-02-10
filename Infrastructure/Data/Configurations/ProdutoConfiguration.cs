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
            
            builder.Property(x => x.PrecoCusto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            builder.Property(x => x.PrecoVenda)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Peso)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Subcategoria)
                .WithMany(s => s.Produtos)
                .HasForeignKey(x => x.SubcategoriaId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Tamanho)
                .WithMany(t => t.Produtos)
                .HasForeignKey(x => x.TamanhoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Cor)
                .WithMany(c => c.Produtos)
                .HasForeignKey(x => x.CorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Genero)
                .WithMany(g => g.Produtos)
                .HasForeignKey(x => x.GeneroId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Marca)
                .WithMany(m => m.Produtos)
                .HasForeignKey(x => x.MarcaId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Fornecedor)
                .WithMany(f => f.Produtos)
                .HasForeignKey(x => x.FornecedorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Estoque)
                .WithOne(e => e.Produto)
                .HasForeignKey<Estoque>(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Produtos");
        }
    }
}
