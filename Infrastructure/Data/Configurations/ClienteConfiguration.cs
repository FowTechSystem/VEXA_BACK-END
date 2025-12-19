using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.NomeCompleto)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CPF)
                .HasMaxLength(14);

            builder.Property(x => x.RG)
                .HasMaxLength(20);

            builder.Property(x => x.CNPJ)
                .HasMaxLength(18);

            builder.Property(x => x.Telefone)
                .HasMaxLength(20);

            builder.Property(x => x.WhatsApp)
                .HasMaxLength(20);

            builder.Property(x => x.Email)
                .HasMaxLength(100);

            builder.Property(x => x.FotoBase64)
                .HasMaxLength(500);

            builder.Property(x => x.DataCadastro)
                .IsRequired();

            builder.HasOne(c => c.Endereco)
                .WithMany(e => e.Clientes)
                .HasForeignKey(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Clientes");
        }
    }
}
