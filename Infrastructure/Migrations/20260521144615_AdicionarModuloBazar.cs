using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarModuloBazar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProdutoId",
                table: "ItensVenda",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "BazarProdutoId",
                table: "ItensVenda",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoItem",
                table: "ItensVenda",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "normal");

            migrationBuilder.CreateTable(
                name: "BazarConsignantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BazarConsignantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BazarConsignantes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BazarRepasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ConsignanteId = table.Column<int>(type: "integer", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DataGeracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pendente"),
                    FormaPagamento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    CriadoPor = table.Column<int>(type: "integer", nullable: false),
                    PagoPor = table.Column<int>(type: "integer", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BazarRepasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BazarRepasses_BazarConsignantes_ConsignanteId",
                        column: x => x.ConsignanteId,
                        principalTable: "BazarConsignantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BazarRepasses_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BazarRepasses_Usuarios_CriadoPor",
                        column: x => x.CriadoPor,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BazarRepasses_Usuarios_PagoPor",
                        column: x => x.PagoPor,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BazarSacolas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ConsignanteId = table.Column<int>(type: "integer", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "date", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BazarSacolas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BazarSacolas_BazarConsignantes_ConsignanteId",
                        column: x => x.ConsignanteId,
                        principalTable: "BazarConsignantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BazarSacolas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BazarProdutos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ConsignanteId = table.Column<int>(type: "integer", nullable: false),
                    SacolaId = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tamanho = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Cor = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    ValorCompra = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ValorVenda = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CodigoEtiqueta = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Disponivel"),
                    DataEntrada = table.Column<DateTime>(type: "date", nullable: false),
                    DataVenda = table.Column<DateTime>(type: "date", nullable: true),
                    DataDevolucao = table.Column<DateTime>(type: "date", nullable: true),
                    VendaId = table.Column<int>(type: "integer", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BazarProdutos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BazarProdutos_BazarConsignantes_ConsignanteId",
                        column: x => x.ConsignanteId,
                        principalTable: "BazarConsignantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BazarProdutos_BazarSacolas_SacolaId",
                        column: x => x.SacolaId,
                        principalTable: "BazarSacolas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BazarProdutos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BazarProdutos_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BazarProdutoFotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BazarProdutoFotos", x => x.Id);
                    table.CheckConstraint("CK_BazarProdutoFotos_Ordem", "\"Ordem\" >= 0 AND \"Ordem\" <= 2");
                    table.ForeignKey(
                        name: "FK_BazarProdutoFotos_BazarProdutos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "BazarProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BazarRepasseItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RepasseId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BazarRepasseItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BazarRepasseItens_BazarProdutos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "BazarProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BazarRepasseItens_BazarRepasses_RepasseId",
                        column: x => x.RepasseId,
                        principalTable: "BazarRepasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_BazarProdutoId",
                table: "ItensVenda",
                column: "BazarProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_BazarConsignantes_EmpresaId_Ativo",
                table: "BazarConsignantes",
                columns: new[] { "EmpresaId", "Ativo" });

            migrationBuilder.CreateIndex(
                name: "IX_BazarConsignantes_EmpresaId_Cpf",
                table: "BazarConsignantes",
                columns: new[] { "EmpresaId", "Cpf" },
                unique: true,
                filter: "\"Cpf\" IS NOT NULL AND \"DeletadoEm\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BazarConsignantes_EmpresaId_Nome",
                table: "BazarConsignantes",
                columns: new[] { "EmpresaId", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_BazarProdutoFotos_ProdutoId_Ordem",
                table: "BazarProdutoFotos",
                columns: new[] { "ProdutoId", "Ordem" });

            migrationBuilder.CreateIndex(
                name: "IX_BazarProdutos_ConsignanteId",
                table: "BazarProdutos",
                column: "ConsignanteId");

            migrationBuilder.CreateIndex(
                name: "IX_BazarProdutos_EmpresaId_CodigoEtiqueta",
                table: "BazarProdutos",
                columns: new[] { "EmpresaId", "CodigoEtiqueta" },
                filter: "\"CodigoEtiqueta\" IS NOT NULL AND \"DeletadoEm\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BazarProdutos_EmpresaId_ConsignanteId_Status",
                table: "BazarProdutos",
                columns: new[] { "EmpresaId", "ConsignanteId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BazarProdutos_EmpresaId_SacolaId",
                table: "BazarProdutos",
                columns: new[] { "EmpresaId", "SacolaId" });

            migrationBuilder.CreateIndex(
                name: "IX_BazarProdutos_EmpresaId_Status",
                table: "BazarProdutos",
                columns: new[] { "EmpresaId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BazarProdutos_SacolaId",
                table: "BazarProdutos",
                column: "SacolaId");

            migrationBuilder.CreateIndex(
                name: "IX_BazarProdutos_VendaId",
                table: "BazarProdutos",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_BazarRepasseItens_ProdutoId",
                table: "BazarRepasseItens",
                column: "ProdutoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BazarRepasseItens_RepasseId",
                table: "BazarRepasseItens",
                column: "RepasseId");

            migrationBuilder.CreateIndex(
                name: "IX_BazarRepasses_ConsignanteId",
                table: "BazarRepasses",
                column: "ConsignanteId");

            migrationBuilder.CreateIndex(
                name: "IX_BazarRepasses_CriadoPor",
                table: "BazarRepasses",
                column: "CriadoPor");

            migrationBuilder.CreateIndex(
                name: "IX_BazarRepasses_EmpresaId_ConsignanteId_Status",
                table: "BazarRepasses",
                columns: new[] { "EmpresaId", "ConsignanteId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BazarRepasses_EmpresaId_Status_DataGeracao",
                table: "BazarRepasses",
                columns: new[] { "EmpresaId", "Status", "DataGeracao" });

            migrationBuilder.CreateIndex(
                name: "IX_BazarRepasses_PagoPor",
                table: "BazarRepasses",
                column: "PagoPor");

            migrationBuilder.CreateIndex(
                name: "IX_BazarSacolas_ConsignanteId",
                table: "BazarSacolas",
                column: "ConsignanteId");

            migrationBuilder.CreateIndex(
                name: "IX_BazarSacolas_EmpresaId_ConsignanteId_DataCadastro",
                table: "BazarSacolas",
                columns: new[] { "EmpresaId", "ConsignanteId", "DataCadastro" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItensVenda_BazarProdutos_BazarProdutoId",
                table: "ItensVenda",
                column: "BazarProdutoId",
                principalTable: "BazarProdutos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensVenda_BazarProdutos_BazarProdutoId",
                table: "ItensVenda");

            migrationBuilder.DropTable(
                name: "BazarProdutoFotos");

            migrationBuilder.DropTable(
                name: "BazarRepasseItens");

            migrationBuilder.DropTable(
                name: "BazarProdutos");

            migrationBuilder.DropTable(
                name: "BazarRepasses");

            migrationBuilder.DropTable(
                name: "BazarSacolas");

            migrationBuilder.DropTable(
                name: "BazarConsignantes");

            migrationBuilder.DropIndex(
                name: "IX_ItensVenda_BazarProdutoId",
                table: "ItensVenda");

            migrationBuilder.DropColumn(
                name: "BazarProdutoId",
                table: "ItensVenda");

            migrationBuilder.DropColumn(
                name: "TipoItem",
                table: "ItensVenda");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoId",
                table: "ItensVenda",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
