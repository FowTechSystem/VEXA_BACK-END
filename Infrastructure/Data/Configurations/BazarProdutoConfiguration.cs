using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations;

public class BazarProdutoConfiguration : IEntityTypeConfiguration<BazarProduto>
{
    public void Configure(EntityTypeBuilder<BazarProduto> builder)
    {
        builder.ToTable("BazarProdutos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Descricao).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Tamanho).HasMaxLength(10);
        builder.Property(x => x.Cor).HasMaxLength(40);
        builder.Property(x => x.ValorCompra).HasColumnType("decimal(10,2)");
        builder.Property(x => x.ValorVenda).HasColumnType("decimal(10,2)");
        builder.Property(x => x.CodigoEtiqueta).HasMaxLength(50);
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(BazarProdutoStatus.Disponivel);
        builder.Property(x => x.DataEntrada).HasColumnType("date");
        builder.Property(x => x.DataVenda).HasColumnType("date");
        builder.Property(x => x.DataDevolucao).HasColumnType("date");

        builder.HasOne(x => x.Consignante)
            .WithMany(c => c.Produtos)
            .HasForeignKey(x => x.ConsignanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Sacola)
            .WithMany(s => s.Produtos)
            .HasForeignKey(x => x.SacolaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Venda)
            .WithMany()
            .HasForeignKey(x => x.VendaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.EmpresaId, x.Status });
        builder.HasIndex(x => new { x.EmpresaId, x.ConsignanteId, x.Status });
        builder.HasIndex(x => new { x.EmpresaId, x.SacolaId });
        builder.HasIndex(x => new { x.EmpresaId, x.CodigoEtiqueta })
            .HasFilter("\"CodigoEtiqueta\" IS NOT NULL AND \"DeletadoEm\" IS NULL");
    }
}
