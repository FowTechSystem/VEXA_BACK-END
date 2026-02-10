using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class TamanhoConfiguration : IEntityTypeConfiguration<Tamanho>
    {
        public void Configure(EntityTypeBuilder<Tamanho> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(20);
            builder.ToTable("Tamanhos");
        }
    }
}
