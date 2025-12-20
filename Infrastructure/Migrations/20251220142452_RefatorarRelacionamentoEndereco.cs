using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefatorarRelacionamentoEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClienteEndereco");

            migrationBuilder.DropTable(
                name: "FornecedorEndereco");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Enderecos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FornecedorId",
                table: "Enderecos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_ClienteId",
                table: "Enderecos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_FornecedorId",
                table: "Enderecos",
                column: "FornecedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enderecos_Clientes_ClienteId",
                table: "Enderecos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enderecos_Fornecedores_FornecedorId",
                table: "Enderecos",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enderecos_Clientes_ClienteId",
                table: "Enderecos");

            migrationBuilder.DropForeignKey(
                name: "FK_Enderecos_Fornecedores_FornecedorId",
                table: "Enderecos");

            migrationBuilder.DropIndex(
                name: "IX_Enderecos_ClienteId",
                table: "Enderecos");

            migrationBuilder.DropIndex(
                name: "IX_Enderecos_FornecedorId",
                table: "Enderecos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Enderecos");

            migrationBuilder.DropColumn(
                name: "FornecedorId",
                table: "Enderecos");

            migrationBuilder.CreateTable(
                name: "ClienteEndereco",
                columns: table => new
                {
                    ClientesId = table.Column<int>(type: "integer", nullable: false),
                    EnderecosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteEndereco", x => new { x.ClientesId, x.EnderecosId });
                    table.ForeignKey(
                        name: "FK_ClienteEndereco_Clientes_ClientesId",
                        column: x => x.ClientesId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClienteEndereco_Enderecos_EnderecosId",
                        column: x => x.EnderecosId,
                        principalTable: "Enderecos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FornecedorEndereco",
                columns: table => new
                {
                    EnderecosId = table.Column<int>(type: "integer", nullable: false),
                    FornecedoresId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedorEndereco", x => new { x.EnderecosId, x.FornecedoresId });
                    table.ForeignKey(
                        name: "FK_FornecedorEndereco_Enderecos_EnderecosId",
                        column: x => x.EnderecosId,
                        principalTable: "Enderecos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FornecedorEndereco_Fornecedores_FornecedoresId",
                        column: x => x.FornecedoresId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClienteEndereco_EnderecosId",
                table: "ClienteEndereco",
                column: "EnderecosId");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorEndereco_FornecedoresId",
                table: "FornecedorEndereco",
                column: "FornecedoresId");
        }
    }
}
