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

            builder.Property(x => x.FotoUrl)
                .HasMaxLength(500);

            builder.Property(x => x.DataCadastro)
                .IsRequired();

            // Configurar Endereco como Owned Entity
            builder.OwnsOne(x => x.Endereco, endereco =>
            {
                endereco.Property(e => e.Rua)
                    .HasColumnName("Endereco_Rua")
                    .HasMaxLength(200);

                endereco.Property(e => e.Numero)
                    .HasColumnName("Endereco_Numero")
                    .HasMaxLength(20);

                endereco.Property(e => e.Bairro)
                    .HasColumnName("Endereco_Bairro")
                    .HasMaxLength(100);

                endereco.Property(e => e.Cidade)
                    .HasColumnName("Endereco_Cidade")
                    .HasMaxLength(100);

                endereco.Property(e => e.Cep)
                    .HasColumnName("Endereco_Cep")
                    .HasMaxLength(10);

                endereco.Property(e => e.Complemento)
                    .HasColumnName("Endereco_Complemento")
                    .HasMaxLength(200);
            });

            builder.ToTable("Clientes");
        }
    }
}
