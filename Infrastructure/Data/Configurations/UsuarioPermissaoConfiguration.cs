using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class UsuarioPermissaoConfiguration : IEntityTypeConfiguration<UsuarioPermissao>
    {
        public void Configure(EntityTypeBuilder<UsuarioPermissao> builder)
        {
            builder.HasKey(up => new { up.UsuarioId, up.PermissaoId });

            builder.HasOne(up => up.Usuario)
                .WithMany(u => u.UsuarioPermissoes)
                .HasForeignKey(up => up.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.Permissao)
                .WithMany(p => p.UsuarioPermissoes)
                .HasForeignKey(up => up.PermissaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("UsuarioPermissoes");
        }
    }
}
