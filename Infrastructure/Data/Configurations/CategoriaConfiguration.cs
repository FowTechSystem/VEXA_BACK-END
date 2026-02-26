using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Descricao).HasMaxLength(500);
            builder.Property(x => x.DataCadastro).IsRequired();
            builder.Property(x => x.Ativo).IsRequired().HasDefaultValue(true);
            builder.ToTable("Categorias");
        }
    }
}
