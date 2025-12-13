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
            builder.Property(x => x.NomeCompleto).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Telefone).IsRequired().HasMaxLength(20);
            builder.OwnsOne(x => x.Endereco, endereco =>
            {
                //endereco.Property(e => e.Rua).HasMaxLength(150);
                //endereco.Property(e => e.CEP).HasMaxLength(8);
            });
        }
    }
}
