using CRMVEXA.Domain.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.Data;

public class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

    public DbSet<TenantCatalog> Tenants => Set<TenantCatalog>();
    public DbSet<UserTenantDirectoryEntry> UserTenantDirectory => Set<UserTenantDirectoryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantCatalog>(e =>
        {
            e.ToTable("Tenants");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            e.Property(x => x.NomeFantasia).HasMaxLength(200);
            e.Property(x => x.Cnpj).HasMaxLength(18);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Telefone).HasMaxLength(20);
            e.Property(x => x.DatabaseName).HasMaxLength(128).IsRequired();
            e.HasIndex(x => x.Cnpj);
        });

        modelBuilder.Entity<UserTenantDirectoryEntry>(e =>
        {
            e.ToTable("UserTenantDirectory");
            e.HasKey(x => x.Id);
            e.Property(x => x.EmailNormalized).HasMaxLength(320).IsRequired();
            e.HasIndex(x => x.EmailNormalized).IsUnique();
            e.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
