using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "Endereco_Bairro",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Endereco_Cep",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Endereco_Cidade",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Endereco_Complemento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Endereco_Numero",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Endereco_Rua",
                table: "Clientes");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Fornecedores",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rua = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Complemento = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClienteEndereco");

            migrationBuilder.DropTable(
                name: "FornecedorEndereco");

            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Fornecedores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Fornecedores",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Bairro",
                table: "Clientes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Cep",
                table: "Clientes",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Cidade",
                table: "Clientes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Complemento",
                table: "Clientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Numero",
                table: "Clientes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Rua",
                table: "Clientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
