using Microsoft.EntityFrameworkCore;
using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Caixa> Caixas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cor> Cores { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<GeneroProduto> GenerosProduto { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Subcategoria> Subcategorias { get; set; }
        public DbSet<Tamanho> Tamanhos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }
        public DbSet<UsuarioPermissao> UsuarioPermissoes { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItensCompra { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
        public DbSet<MovimentacaoCaixa> MovimentacoesCaixa { get; set; }
        public DbSet<MovimentacaoCaixaManual> MovimentacoesCaixaManuais { get; set; }
        public DbSet<Condicional> Condicionais { get; set; }
        public DbSet<ItemCondicional> ItensCondicional { get; set; }
        public DbSet<Catalogo> Catalogos { get; set; }
        public DbSet<ItemCatalogo> ItensCatalogo { get; set; }

        public DbSet<PermissaoCatalogo> PermissoesCatalogo { get; set; }
        public DbSet<PerfilAcesso> PerfisAcesso { get; set; }
        public DbSet<PerfilPermissaoCatalogo> PerfilPermissoesCatalogo { get; set; }
        public DbSet<ParametroSistema> ParametrosSistema { get; set; }
        public DbSet<SolicitacaoAprovacao> SolicitacoesAprovacao { get; set; }
        public DbSet<AuditoriaAcao> AuditoriaAcoes { get; set; }

        public DbSet<BazarConsignante> BazarConsignantes { get; set; }
        public DbSet<BazarSacola> BazarSacolas { get; set; }
        public DbSet<BazarProduto> BazarProdutos { get; set; }
        public DbSet<BazarProdutoFoto> BazarProdutoFotos { get; set; }
        public DbSet<BazarRepasse> BazarRepasses { get; set; }
        public DbSet<BazarRepasseItem> BazarRepasseItens { get; set; }

        public DbSet<EmpresaConfiguracao> EmpresaConfiguracoes { get; set; }
        public DbSet<EmpresaModulo> EmpresaModulos { get; set; }

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

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Enderecos)
                .WithOne(e => e.Usuario)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Relacionamentos Empresa (multi-tenancy) - apenas para entidades que têm EmpresaId mas NÃO têm relação configurada por convenção
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType == typeof(Empresa)) continue;
                if (entityType.FindProperty("EmpresaId") == null) continue;
                // Pular se já tem FK para Empresa (convenção ou configuração)
                if (entityType.GetForeignKeys().Any(fk => fk.PrincipalEntityType.ClrType == typeof(Empresa))) continue;

                var hasEmpresaNav = entityType.ClrType.GetProperty("Empresa", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) != null;
                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(Empresa), hasEmpresaNav ? "Empresa" : null)
                    .WithMany()
                    .HasForeignKey("EmpresaId")
                    .OnDelete(DeleteBehavior.Restrict);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
