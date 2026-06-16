using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioCamposCompletosEPermissoes3 : Migration
    {
        /// <inheritdoc />
        /// <remarks>
        /// Duplica a 20260218162130 (mesmas colunas/tabelas). Reaproveita o mesmo SQL
        /// idempotente para não quebrar bancos novos com "coluna já existe".
        /// </remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UsuarioCamposCompletosEPermissoes.AplicarIdempotente(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
