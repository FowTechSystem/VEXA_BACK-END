using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class CondicionalConfiguration : IEntityTypeConfiguration<Condicional>
{
    public void Configure(EntityTypeBuilder<Condicional> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(20).HasDefaultValue("pendente");
        builder.Property(x => x.ValorTotal).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Data).IsRequired();

        builder.HasMany(c => c.Itens)
            .WithOne()
            .HasForeignKey("CondicionalId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Cliente)
            .WithMany()
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Usuario)
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Condicionais");
    }
}
