using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class CaixaConfiguration : IEntityTypeConfiguration<Caixa>
    {
        public void Configure(EntityTypeBuilder<Caixa> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DataAbertura).IsRequired();
            builder.Property(x => x.SaldoInicial)
                   .HasColumnType("decimal(18,2)");
            builder.Property(x => x.SaldoFinal)
                   .HasColumnType("decimal(18,2)");
            builder.Property(x => x.Aberto).IsRequired();
        }
    }
}
