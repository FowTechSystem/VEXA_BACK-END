using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Login).IsRequired().HasMaxLength(80);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CPF).HasMaxLength(14);
            builder.Property(x => x.CNPJ).HasMaxLength(18);
            builder.Property(x => x.RG).HasMaxLength(20);
            builder.Property(x => x.Telefone).HasMaxLength(20);
            builder.Property(x => x.WhatsApp).HasMaxLength(20);
            builder.Property(x => x.Perfil).HasMaxLength(50);
            builder.Property(x => x.Departamento).HasMaxLength(100);
            builder.Property(x => x.Observacoes).HasMaxLength(2000);
            builder.Property(x => x.Foto).HasColumnType("bytea");
            builder.ToTable("Usuarios");
        }
    }
}
