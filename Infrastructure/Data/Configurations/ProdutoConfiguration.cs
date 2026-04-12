using System.Linq;
using System.Text.Json;
using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        private static readonly JsonSerializerOptions ListaJson = new() { WriteIndented = false };

        private static readonly ValueConverter<List<string>, string> ListaStringJson = new(
            v => JsonSerializer.Serialize(v ?? new List<string>(), ListaJson),
            v => string.IsNullOrWhiteSpace(v) || v == "null"
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(v, ListaJson) ?? new List<string>());

        private static readonly ValueComparer<List<string>> ListaStringComparer = new(
            (a, b) => (a == null && b == null) || (a != null && b != null && a.SequenceEqual(b)),
            c => c == null ? 0 : c.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode(StringComparison.Ordinal))),
            c => c == null ? new List<string>() : c.ToList());

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

            builder.Property(x => x.PrecoPromocional)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.EmPromocao)
                .IsRequired();

            builder.Property(x => x.Observacoes)
                .HasMaxLength(4000);

            builder.Property(x => x.Peso)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Foto1).HasColumnType("bytea");
            builder.Property(x => x.Foto2).HasColumnType("bytea");
            builder.Property(x => x.Foto3).HasColumnType("bytea");
            builder.Property(x => x.Foto4).HasColumnType("bytea");
            builder.Property(x => x.Foto5).HasColumnType("bytea");

            builder.Property(x => x.Cores)
                .HasColumnType("jsonb")
                .HasConversion(ListaStringJson)
                .Metadata.SetValueComparer(ListaStringComparer);

            builder.Property(x => x.Tamanhos)
                .HasColumnType("jsonb")
                .HasConversion(ListaStringJson)
                .Metadata.SetValueComparer(ListaStringComparer);

            builder.Property(x => x.TipoTamanho).HasMaxLength(32);
            builder.Property(x => x.Material).HasMaxLength(120);

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
