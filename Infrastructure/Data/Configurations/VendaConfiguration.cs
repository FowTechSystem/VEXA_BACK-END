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
            
            builder.Property(x => x.CodigoVenda)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(x => x.FormaPagamento)
                .IsRequired()
                .HasMaxLength(30);
            
            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Concluída");
            
            builder.Property(x => x.ValorTotal)
                .HasColumnType("decimal(18,2)");
            
            builder.Property(x => x.Desconto)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(x => x.Itens)
                .WithOne()
                .HasForeignKey("VendaId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Vendas");
        }
    }
}
