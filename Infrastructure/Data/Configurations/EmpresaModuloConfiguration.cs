using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class EmpresaModuloConfiguration : IEntityTypeConfiguration<EmpresaModulo>
{
    public void Configure(EntityTypeBuilder<EmpresaModulo> builder)
    {
        builder.ToTable("company_modules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ModuleKey).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => new { x.EmpresaId, x.ModuleKey }).IsUnique();
        builder.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.AtualizadoPor).WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.SetNull);
    }
}
