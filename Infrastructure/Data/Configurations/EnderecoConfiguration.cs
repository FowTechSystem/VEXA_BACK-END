using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.Property(x => x.Rua)
                .HasMaxLength(200);

            builder.Property(x => x.Numero)
                .HasMaxLength(20);

            builder.Property(x => x.Bairro)
                .HasMaxLength(100);

            builder.Property(x => x.Cidade)
                .HasMaxLength(100);

            builder.Property(x => x.Cep)
                .HasMaxLength(10);

            builder.Property(x => x.Complemento)
                .HasMaxLength(200);

            builder.ToTable("Enderecos");
        }
    }
}
