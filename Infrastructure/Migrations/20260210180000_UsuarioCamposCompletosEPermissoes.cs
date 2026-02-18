using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    public partial class UsuarioCamposCompletosEPermissoes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Novos campos em Usuarios
            migrationBuilder.AddColumn<bool>("Ativo", "Usuarios", "boolean", nullable: false, defaultValue: true);
            migrationBuilder.AddColumn<string>("CPF", "Usuarios", "character varying(14)", maxLength: 14, nullable: true);
            migrationBuilder.AddColumn<string>("Departamento", "Usuarios", "character varying(100)", maxLength: 100, nullable: true);
            migrationBuilder.AddColumn<DateTime>("DataNascimento", "Usuarios", "timestamp without time zone", nullable: true);
            migrationBuilder.AddColumn<string>("Observacoes", "Usuarios", "character varying(2000)", maxLength: 2000, nullable: true);
            migrationBuilder.AddColumn<string>("Perfil", "Usuarios", "character varying(50)", maxLength: 50, nullable: true);
            migrationBuilder.AddColumn<string>("RG", "Usuarios", "character varying(20)", maxLength: 20, nullable: true);
            migrationBuilder.AddColumn<string>("Telefone", "Usuarios", "character varying(20)", maxLength: 20, nullable: true);
            migrationBuilder.AddColumn<string>("WhatsApp", "Usuarios", "character varying(20)", maxLength: 20, nullable: true);

            // Estado em Enderecos
            migrationBuilder.AddColumn<string>("Estado", "Enderecos", "character varying(2)", maxLength: 2, nullable: true);
            migrationBuilder.AddColumn<int>("UsuarioId", "Enderecos", "integer", nullable: true);

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

            migrationBuilder.CreateIndex("IX_Permissoes_Codigo", "Permissoes", "Codigo", unique: true);

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

            migrationBuilder.CreateIndex("IX_UsuarioPermissoes_PermissaoId", "UsuarioPermissoes", "PermissaoId");

            // FK Enderecos -> Usuarios
            migrationBuilder.CreateIndex("IX_Enderecos_UsuarioId", "Enderecos", "UsuarioId");
            migrationBuilder.AddForeignKey(
                name: "FK_Enderecos_Usuarios_UsuarioId",
                table: "Enderecos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Seed permissões do formulário
            migrationBuilder.Sql(@"
                INSERT INTO ""Permissoes"" (""Nome"", ""Codigo"") VALUES
                ('Visualizar relatórios', 'VISUALIZAR_RELATORIOS'),
                ('Gerenciar clientes', 'GERENCIAR_CLIENTES'),
                ('Acesso financeiro', 'ACESSO_FINANCEIRO'),
                ('Configurações do sistema', 'CONFIGURACOES_SISTEMA');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Enderecos_Usuarios_UsuarioId", "Enderecos");
            migrationBuilder.DropIndex("IX_Enderecos_UsuarioId", "Enderecos");
            migrationBuilder.DropTable("UsuarioPermissoes");
            migrationBuilder.DropTable("Permissoes");

            migrationBuilder.DropColumn("Ativo", "Usuarios");
            migrationBuilder.DropColumn("CPF", "Usuarios");
            migrationBuilder.DropColumn("Departamento", "Usuarios");
            migrationBuilder.DropColumn("DataNascimento", "Usuarios");
            migrationBuilder.DropColumn("Observacoes", "Usuarios");
            migrationBuilder.DropColumn("Perfil", "Usuarios");
            migrationBuilder.DropColumn("RG", "Usuarios");
            migrationBuilder.DropColumn("Telefone", "Usuarios");
            migrationBuilder.DropColumn("WhatsApp", "Usuarios");

            migrationBuilder.DropColumn("Estado", "Enderecos");
            migrationBuilder.DropColumn("UsuarioId", "Enderecos");
        }
    }
}
