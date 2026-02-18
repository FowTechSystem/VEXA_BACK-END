using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioCamposCompletosEPermissoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Usuarios: novos campos
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true);
            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "Usuarios",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Departamento",
                table: "Usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "Usuarios",
                type: "timestamp without time zone",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Usuarios",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Perfil",
                table: "Usuarios",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "RG",
                table: "Usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "WhatsApp",
                table: "Usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            // Enderecos: Estado e UsuarioId
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Enderecos",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Enderecos",
                type: "integer",
                nullable: true);
            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_UsuarioId",
                table: "Enderecos",
                column: "UsuarioId");
            migrationBuilder.AddForeignKey(
                name: "FK_Enderecos_Usuarios_UsuarioId",
                table: "Enderecos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Tabela Permissoes
            migrationBuilder.CreateTable(
                name: "Permissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Permissoes", x => x.Id));
            migrationBuilder.CreateIndex(
                name: "IX_Permissoes_Codigo",
                table: "Permissoes",
                column: "Codigo",
                unique: true);

            // Tabela UsuarioPermissoes (N:N)
            migrationBuilder.CreateTable(
                name: "UsuarioPermissoes",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    PermissaoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPermissoes", x => new { x.UsuarioId, x.PermissaoId });
                    table.ForeignKey(
                        name: "FK_UsuarioPermissoes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioPermissoes_Permissoes_PermissaoId",
                        column: x => x.PermissaoId,
                        principalTable: "Permissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissoes_PermissaoId",
                table: "UsuarioPermissoes",
                column: "PermissaoId");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorTotal",
                table: "Vendas",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Vendas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Concluída",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "Desconto",
                table: "Vendas",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Peso",
                table: "Produtos",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHora",
                table: "MovimentacoesEstoque",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHora",
                table: "MovimentacoesCaixa",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoUnitario",
                table: "ItensVenda",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Compras",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Concluída",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHora",
                table: "Compras",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Clientes_ClienteId",
                table: "Vendas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Usuarios_UsuarioId",
                table: "Vendas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Clientes_ClienteId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Usuarios_UsuarioId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Enderecos_Usuarios_UsuarioId",
                table: "Enderecos");
            migrationBuilder.DropIndex(
                name: "IX_Enderecos_UsuarioId",
                table: "Enderecos");
            migrationBuilder.DropTable(
                name: "UsuarioPermissoes");
            migrationBuilder.DropTable(
                name: "Permissoes");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Enderecos");
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Enderecos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Usuarios");
            migrationBuilder.DropColumn(
                name: "CPF",
                table: "Usuarios");
            migrationBuilder.DropColumn(
                name: "Departamento",
                table: "Usuarios");
            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Usuarios");
            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Usuarios");
            migrationBuilder.DropColumn(
                name: "Perfil",
                table: "Usuarios");
            migrationBuilder.DropColumn(
                name: "RG",
                table: "Usuarios");
            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Usuarios");
            migrationBuilder.DropColumn(
                name: "WhatsApp",
                table: "Usuarios");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorTotal",
                table: "Vendas",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Vendas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Concluída");

            migrationBuilder.AlterColumn<decimal>(
                name: "Desconto",
                table: "Vendas",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Peso",
                table: "Produtos",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHora",
                table: "MovimentacoesEstoque",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHora",
                table: "MovimentacoesCaixa",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoUnitario",
                table: "ItensVenda",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Compras",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Concluída");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHora",
                table: "Compras",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
