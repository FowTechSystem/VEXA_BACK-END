using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Caixas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataAbertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SaldoInicial = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SaldoFinal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    Aberto = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caixas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeCompleto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    WhatsApp = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Endereco_Rua = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Endereco_Numero = table.Column<string>(type: "text", nullable: false),
                    Endereco_Bairro = table.Column<string>(type: "text", nullable: false),
                    Endereco_Cidade = table.Column<string>(type: "text", nullable: false),
                    Endereco_CEP = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Preferencias = table.Column<string>(type: "text", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RazaoSocial = table.Column<string>(type: "text", nullable: true),
                    CNPJ = table.Column<string>(type: "text", nullable: true),
                    NomeRepresentante = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    WhatsApp = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Endereco = table.Column<string>(type: "text", nullable: true),
                    CategoriaProdutos = table.Column<string>(type: "text", nullable: true),
                    CondicaoPagamento = table.Column<string>(type: "text", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: false),
                    Subcategoria = table.Column<string>(type: "text", nullable: true),
                    Tamanho = table.Column<string>(type: "text", nullable: true),
                    Cor = table.Column<string>(type: "text", nullable: true),
                    Genero = table.Column<string>(type: "text", nullable: false),
                    Marca = table.Column<string>(type: "text", nullable: true),
                    FornecedorId = table.Column<int>(type: "integer", nullable: true),
                    PrecoCusto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CodigoBarras = table.Column<string>(type: "text", nullable: true),
                    QuantidadeEstoque = table.Column<int>(type: "integer", nullable: false),
                    EstoqueMinimo = table.Column<int>(type: "integer", nullable: false),
                    LocalizacaoEstoque = table.Column<string>(type: "text", nullable: true),
                    Foto1 = table.Column<string>(type: "text", nullable: true),
                    Foto2 = table.Column<string>(type: "text", nullable: true),
                    Foto3 = table.Column<string>(type: "text", nullable: true),
                    Foto4 = table.Column<string>(type: "text", nullable: true),
                    Foto5 = table.Column<string>(type: "text", nullable: true),
                    DescricaoCurta = table.Column<string>(type: "text", nullable: true),
                    Peso = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Login = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    PodeVender = table.Column<bool>(type: "boolean", nullable: false),
                    PodeEditarEstoque = table.Column<bool>(type: "boolean", nullable: false),
                    PodeEditarProduto = table.Column<bool>(type: "boolean", nullable: false),
                    PodeAbrirFecharCaixa = table.Column<bool>(type: "boolean", nullable: false),
                    PodeEmitirRelatorios = table.Column<bool>(type: "boolean", nullable: false),
                    PodeCadastrarFornecedores = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoVenda = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: true),
                    Desconto = table.Column<decimal>(type: "numeric", nullable: false),
                    FormaPagamento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    DataHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensVenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendaId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensVenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_VendaId",
                table: "ItensVenda",
                column: "VendaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Caixas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "ItensVenda");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Vendas");
        }
    }
}
