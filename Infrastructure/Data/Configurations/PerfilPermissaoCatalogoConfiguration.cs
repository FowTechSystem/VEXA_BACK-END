using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class PerfilPermissaoCatalogoConfiguration : IEntityTypeConfiguration<PerfilPermissaoCatalogo>
{
    public void Configure(EntityTypeBuilder<PerfilPermissaoCatalogo> builder)
    {
        builder.ToTable("PerfilPermissoesCatalogo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nivel).IsRequired().HasMaxLength(20);
        builder.HasIndex(x => new { x.PerfilAcessoId, x.PermissaoCatalogoId }).IsUnique();
        builder.HasOne(x => x.PerfilAcesso).WithMany(x => x.PerfilPermissoes).HasForeignKey(x => x.PerfilAcessoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.PermissaoCatalogo).WithMany(x => x.PerfilPermissoes).HasForeignKey(x => x.PermissaoCatalogoId).OnDelete(DeleteBehavior.Cascade);
    }
}
