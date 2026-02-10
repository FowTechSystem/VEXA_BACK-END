using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class GeneroProdutoConfiguration : IEntityTypeConfiguration<GeneroProduto>
    {
        public void Configure(EntityTypeBuilder<GeneroProduto> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(50);
            builder.ToTable("GenerosProduto");
        }
    }
}
