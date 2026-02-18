using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(200);
            builder.Property(x => x.NomeFantasia).HasMaxLength(200);
            builder.Property(x => x.CNPJ).HasMaxLength(18);
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.Telefone).HasMaxLength(20);
            builder.ToTable("Empresas");
        }
    }
}
