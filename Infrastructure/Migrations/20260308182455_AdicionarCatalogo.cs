using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCatalogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    CriadorId = table.Column<int>(type: "integer", nullable: false),
                    CriadorNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CriadorWhatsApp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmpresaNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalogos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalogos_Usuarios_CriadorId",
                        column: x => x.CriadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensCatalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CatalogoId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Preco = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCatalogo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensCatalogo_Catalogos_CatalogoId",
                        column: x => x.CatalogoId,
                        principalTable: "Catalogos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensCatalogo_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalogos_CriadorId",
                table: "Catalogos",
                column: "CriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogos_EmpresaId_Slug",
                table: "Catalogos",
                columns: new[] { "EmpresaId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_Catalogos_Slug",
                table: "Catalogos",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCatalogo_CatalogoId",
                table: "ItensCatalogo",
                column: "CatalogoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCatalogo_ProdutoId",
                table: "ItensCatalogo",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensCatalogo");

            migrationBuilder.DropTable(
                name: "Catalogos");
        }
    }
}
