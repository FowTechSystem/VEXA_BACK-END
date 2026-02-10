using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefatorarProdutoComTabelasRelacionadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Criar tabelas novas
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Categorias", x => x.Id));

            migrationBuilder.CreateTable(
                name: "GenerosProduto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_GenerosProduto", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Marcas", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Cores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Cores", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Tamanhos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Tamanhos", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Subcategorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcategorias_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // 2. Inserir dados iniciais
            migrationBuilder.Sql(@"INSERT INTO ""Categorias"" (""Nome"") VALUES ('Geral');");
            migrationBuilder.Sql(@"INSERT INTO ""GenerosProduto"" (""Nome"") VALUES ('Masculino'), ('Feminino'), ('Unissex');");

            // 3. Migrar categorias e gêneros distintos dos produtos
            migrationBuilder.Sql(@"
                INSERT INTO ""Categorias"" (""Nome"")
                SELECT DISTINCT p.""Categoria"" FROM ""Produtos"" p
                WHERE p.""Categoria"" IS NOT NULL AND p.""Categoria"" != ''
                AND NOT EXISTS (SELECT 1 FROM ""Categorias"" c WHERE c.""Nome"" = p.""Categoria"");
            ");

            migrationBuilder.Sql(@"
                INSERT INTO ""GenerosProduto"" (""Nome"")
                SELECT DISTINCT p.""Genero"" FROM ""Produtos"" p
                WHERE p.""Genero"" IS NOT NULL AND p.""Genero"" != ''
                AND NOT EXISTS (SELECT 1 FROM ""GenerosProduto"" g WHERE g.""Nome"" = p.""Genero"");
            ");

            // 4. Migrar Marcas, Cores, Tamanhos
            migrationBuilder.Sql(@"
                INSERT INTO ""Marcas"" (""Nome"")
                SELECT DISTINCT p.""Marca"" FROM ""Produtos"" p
                WHERE p.""Marca"" IS NOT NULL AND p.""Marca"" != ''
                AND NOT EXISTS (SELECT 1 FROM ""Marcas"" m WHERE m.""Nome"" = p.""Marca"");
            ");

            migrationBuilder.Sql(@"
                INSERT INTO ""Cores"" (""Nome"")
                SELECT DISTINCT p.""Cor"" FROM ""Produtos"" p
                WHERE p.""Cor"" IS NOT NULL AND p.""Cor"" != ''
                AND NOT EXISTS (SELECT 1 FROM ""Cores"" c WHERE c.""Nome"" = p.""Cor"");
            ");

            migrationBuilder.Sql(@"
                INSERT INTO ""Tamanhos"" (""Nome"")
                SELECT DISTINCT p.""Tamanho"" FROM ""Produtos"" p
                WHERE p.""Tamanho"" IS NOT NULL AND p.""Tamanho"" != ''
                AND NOT EXISTS (SELECT 1 FROM ""Tamanhos"" t WHERE t.""Nome"" = p.""Tamanho"");
            ");

            // 5. Adicionar novas colunas em Produtos
            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Produtos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CorId",
                table: "Produtos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneroId",
                table: "Produtos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarcaId",
                table: "Produtos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubcategoriaId",
                table: "Produtos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TamanhoId",
                table: "Produtos",
                type: "integer",
                nullable: true);

            // 6. Atualizar Produtos com CategoriaId e GeneroId
            migrationBuilder.Sql(@"
                UPDATE ""Produtos"" p
                SET ""CategoriaId"" = (SELECT c.""Id"" FROM ""Categorias"" c WHERE c.""Nome"" = p.""Categoria"" LIMIT 1)
                WHERE p.""Categoria"" IS NOT NULL AND p.""Categoria"" != '';
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Produtos"" p
                SET ""GeneroId"" = (SELECT g.""Id"" FROM ""GenerosProduto"" g WHERE g.""Nome"" = p.""Genero"" LIMIT 1)
                WHERE p.""Genero"" IS NOT NULL AND p.""Genero"" != '';
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Produtos"" SET ""CategoriaId"" = 1 WHERE ""CategoriaId"" IS NULL;
            ");
            migrationBuilder.Sql(@"
                UPDATE ""Produtos"" SET ""GeneroId"" = (SELECT ""Id"" FROM ""GenerosProduto"" WHERE ""Nome"" = 'Unissex' LIMIT 1) WHERE ""GeneroId"" IS NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Produtos"" p SET ""MarcaId"" = (SELECT m.""Id"" FROM ""Marcas"" m WHERE m.""Nome"" = p.""Marca"" LIMIT 1) WHERE p.""Marca"" IS NOT NULL AND p.""Marca"" != '';
                UPDATE ""Produtos"" p SET ""CorId"" = (SELECT c.""Id"" FROM ""Cores"" c WHERE c.""Nome"" = p.""Cor"" LIMIT 1) WHERE p.""Cor"" IS NOT NULL AND p.""Cor"" != '';
                UPDATE ""Produtos"" p SET ""TamanhoId"" = (SELECT t.""Id"" FROM ""Tamanhos"" t WHERE t.""Nome"" = p.""Tamanho"" LIMIT 1) WHERE p.""Tamanho"" IS NOT NULL AND p.""Tamanho"" != '';
            ");

            // 7. Criar tabela Estoque ANTES de dropar colunas
            migrationBuilder.CreateTable(
                name: "Estoques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    EstoqueMinimo = table.Column<int>(type: "integer", nullable: false),
                    LocalizacaoEstoque = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estoques_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_ProdutoId",
                table: "Estoques",
                column: "ProdutoId",
                unique: true);

            // 8. Migrar dados para Estoque (antes de dropar colunas)
            migrationBuilder.Sql(@"
                INSERT INTO ""Estoques"" (""ProdutoId"", ""Quantidade"", ""EstoqueMinimo"", ""LocalizacaoEstoque"")
                SELECT ""Id"", ""QuantidadeEstoque"", COALESCE(""EstoqueMinimo"", 1), ""LocalizacaoEstoque""
                FROM ""Produtos"";
            ");

            // 9. Remover FK Fornecedor temporariamente, dropar colunas antigas
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos");

            migrationBuilder.DropColumn(name: "Categoria", table: "Produtos");
            migrationBuilder.DropColumn(name: "Subcategoria", table: "Produtos");
            migrationBuilder.DropColumn(name: "Tamanho", table: "Produtos");
            migrationBuilder.DropColumn(name: "Cor", table: "Produtos");
            migrationBuilder.DropColumn(name: "Genero", table: "Produtos");
            migrationBuilder.DropColumn(name: "Marca", table: "Produtos");
            migrationBuilder.DropColumn(name: "QuantidadeEstoque", table: "Produtos");
            migrationBuilder.DropColumn(name: "EstoqueMinimo", table: "Produtos");
            migrationBuilder.DropColumn(name: "LocalizacaoEstoque", table: "Produtos");

            // 10. Tornar CategoriaId e GeneroId obrigatórios
            migrationBuilder.AlterColumn<int>(
                name: "CategoriaId",
                table: "Produtos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GeneroId",
                table: "Produtos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // 11. Criar índices e FKs
            migrationBuilder.CreateIndex(
                name: "IX_Subcategorias_CategoriaId",
                table: "Subcategorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_GeneroId",
                table: "Produtos",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_SubcategoriaId",
                table: "Produtos",
                column: "SubcategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TamanhoId",
                table: "Produtos",
                column: "TamanhoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CorId",
                table: "Produtos",
                column: "CorId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_MarcaId",
                table: "Produtos",
                column: "MarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_GenerosProduto_GeneroId",
                table: "Produtos",
                column: "GeneroId",
                principalTable: "GenerosProduto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Subcategorias_SubcategoriaId",
                table: "Produtos",
                column: "SubcategoriaId",
                principalTable: "Subcategorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Tamanhos_TamanhoId",
                table: "Produtos",
                column: "TamanhoId",
                principalTable: "Tamanhos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Cores_CorId",
                table: "Produtos",
                column: "CorId",
                principalTable: "Cores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Marcas_MarcaId",
                table: "Produtos",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_FornecedorId",
                table: "Produtos",
                column: "FornecedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos");
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos");
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_GenerosProduto_GeneroId",
                table: "Produtos");
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Subcategorias_SubcategoriaId",
                table: "Produtos");
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Tamanhos_TamanhoId",
                table: "Produtos");
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Cores_CorId",
                table: "Produtos");
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Marcas_MarcaId",
                table: "Produtos");

            migrationBuilder.DropTable(name: "Estoques");
            migrationBuilder.DropTable(name: "Subcategorias");
            migrationBuilder.DropTable(name: "Cores");
            migrationBuilder.DropTable(name: "Tamanhos");
            migrationBuilder.DropTable(name: "Marcas");
            migrationBuilder.DropTable(name: "GenerosProduto");
            migrationBuilder.DropTable(name: "Categorias");

            migrationBuilder.AddColumn<string>(name: "Categoria", table: "Produtos", type: "text", nullable: false, defaultValue: "Geral");
            migrationBuilder.AddColumn<string>(name: "Subcategoria", table: "Produtos", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Tamanho", table: "Produtos", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Cor", table: "Produtos", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Genero", table: "Produtos", type: "text", nullable: false, defaultValue: "Unissex");
            migrationBuilder.AddColumn<string>(name: "Marca", table: "Produtos", type: "text", nullable: true);
            migrationBuilder.AddColumn<int>(name: "QuantidadeEstoque", table: "Produtos", type: "integer", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "EstoqueMinimo", table: "Produtos", type: "integer", nullable: false, defaultValue: 1);
            migrationBuilder.AddColumn<string>(name: "LocalizacaoEstoque", table: "Produtos", type: "text", nullable: true);

            migrationBuilder.DropColumn(name: "CategoriaId", table: "Produtos");
            migrationBuilder.DropColumn(name: "SubcategoriaId", table: "Produtos");
            migrationBuilder.DropColumn(name: "TamanhoId", table: "Produtos");
            migrationBuilder.DropColumn(name: "CorId", table: "Produtos");
            migrationBuilder.DropColumn(name: "GeneroId", table: "Produtos");
            migrationBuilder.DropColumn(name: "MarcaId", table: "Produtos");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
