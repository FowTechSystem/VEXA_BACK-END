using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class EmpresaConfiguracaoConfiguration : IEntityTypeConfiguration<EmpresaConfiguracao>
{
    public void Configure(EntityTypeBuilder<EmpresaConfiguracao> builder)
    {
        builder.ToTable("company_settings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LogoPath).HasMaxLength(500);
        builder.Property(x => x.LogoFilename).HasMaxLength(255);
        builder.Property(x => x.PrimaryColor).IsRequired().HasMaxLength(7);
        builder.Property(x => x.SecondaryColor).IsRequired().HasMaxLength(7);
        builder.Property(x => x.DefaultTheme)
            .HasConversion<string>()
            .HasMaxLength(10)
            .HasDefaultValue(TemaPadraoEmpresa.Dark);
        builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.EmpresaId).IsUnique();
        builder.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.AtualizadoPor).WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.SetNull);
    }
}
