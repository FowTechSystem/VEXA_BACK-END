using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoque>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TipoMovimentacao)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Motivo)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne<Produto>()
                .WithMany()
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("MovimentacoesEstoque");
        }
    }
}
