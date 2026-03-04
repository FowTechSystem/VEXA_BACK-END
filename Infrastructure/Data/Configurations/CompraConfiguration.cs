using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class CompraConfiguration : IEntityTypeConfiguration<Compra>
    {
        public void Configure(EntityTypeBuilder<Compra> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CodigoCompra)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("pendente");

            builder.Property(x => x.DataEntregaPrevista);

            builder.Property(x => x.ValorTotal)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(x => x.Itens)
                .WithOne()
                .HasForeignKey("CompraId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Fornecedor>()
                .WithMany()
                .HasForeignKey(x => x.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Compras");
        }
    }
}
