using CRMVEXA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMVEXA.Infrastructure.Data.Configurations
{
    public class SubcategoriaConfiguration : IEntityTypeConfiguration<Subcategoria>
    {
        public void Configure(EntityTypeBuilder<Subcategoria> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(100);
            builder.HasOne(x => x.Categoria).WithMany(c => c.Subcategorias).HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.Restrict);
            builder.ToTable("Subcategorias");
        }
    }
}
