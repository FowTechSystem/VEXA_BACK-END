using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class MovimentacaoCaixaConfiguration : IEntityTypeConfiguration<MovimentacaoCaixa>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoCaixa> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Tipo)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.FormaPagamento)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Valor)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasOne<Caixa>()
                .WithMany()
                .HasForeignKey(x => x.CaixaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("MovimentacoesCaixa");
        }
    }
}
