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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<Endereco>().ToTable("Enderecos");

            modelBuilder.Entity<Fornecedor>()
                .HasMany(f => f.Enderecos)
                .WithMany(e => e.Fornecedores)
                .UsingEntity(j => j.ToTable("FornecedorEndereco"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
