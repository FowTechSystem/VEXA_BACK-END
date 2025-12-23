using Microsoft.EntityFrameworkCore;
using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Caixa> Caixas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fornecedor>()
                .HasMany(f => f.Enderecos)
                .WithOne(e => e.Fornecedor)
                .HasForeignKey(e => e.FornecedorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Enderecos)
                .WithOne(e => e.Cliente)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
