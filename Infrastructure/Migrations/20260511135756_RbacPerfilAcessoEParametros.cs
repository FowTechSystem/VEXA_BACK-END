using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RbacPerfilAcessoEParametros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerfilAcessoId",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditoriaAcoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Acao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RecursoId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AntesJson = table.Column<string>(type: "jsonb", nullable: true),
                    DepoisJson = table.Column<string>(type: "jsonb", nullable: true),
                    EnderecoIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaAcoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriaAcoes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditoriaAcoes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParametrosSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Chave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Rotulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AtualizadoPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosSistema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParametrosSistema_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParametrosSistema_Usuarios_AtualizadoPorUsuarioId",
                        column: x => x.AtualizadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PerfisAcesso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Rotulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PadraoNovoUsuario = table.Column<bool>(type: "boolean", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisAcesso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfisAcesso_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissoesCatalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Modulo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Acao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Recurso = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Chave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rotulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissoesCatalogo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolicitacoesAprovacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    SolicitadoPorUsuarioId = table.Column<int>(type: "integer", nullable: false),
                    AprovadoPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    PermissaoChave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContextoJson = table.Column<string>(type: "jsonb", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Motivo = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RespostaNota = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RespondidoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesAprovacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitacoesAprovacao_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesAprovacao_Usuarios_AprovadoPorUsuarioId",
                        column: x => x.AprovadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SolicitacoesAprovacao_Usuarios_SolicitadoPorUsuarioId",
                        column: x => x.SolicitadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PerfilPermissoesCatalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilAcessoId = table.Column<int>(type: "integer", nullable: false),
                    PermissaoCatalogoId = table.Column<int>(type: "integer", nullable: false),
                    Nivel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilPermissoesCatalogo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilPermissoesCatalogo_PerfisAcesso_PerfilAcessoId",
                        column: x => x.PerfilAcessoId,
                        principalTable: "PerfisAcesso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilPermissoesCatalogo_PermissoesCatalogo_PermissaoCatalo~",
                        column: x => x.PermissaoCatalogoId,
                        principalTable: "PermissoesCatalogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PerfilAcessoId",
                table: "Usuarios",
                column: "PerfilAcessoId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaAcoes_CriadoEm",
                table: "AuditoriaAcoes",
                column: "CriadoEm");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaAcoes_EmpresaId_Acao",
                table: "AuditoriaAcoes",
                columns: new[] { "EmpresaId", "Acao" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaAcoes_UsuarioId",
                table: "AuditoriaAcoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosSistema_AtualizadoPorUsuarioId",
                table: "ParametrosSistema",
                column: "AtualizadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosSistema_EmpresaId_Chave",
                table: "ParametrosSistema",
                columns: new[] { "EmpresaId", "Chave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilPermissoesCatalogo_PerfilAcessoId_PermissaoCatalogoId",
                table: "PerfilPermissoesCatalogo",
                columns: new[] { "PerfilAcessoId", "PermissaoCatalogoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilPermissoesCatalogo_PermissaoCatalogoId",
                table: "PerfilPermissoesCatalogo",
                column: "PermissaoCatalogoId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfisAcesso_EmpresaId_Nome",
                table: "PerfisAcesso",
                columns: new[] { "EmpresaId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissoesCatalogo_Chave",
                table: "PermissoesCatalogo",
                column: "Chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissoesCatalogo_Modulo_Acao_Recurso",
                table: "PermissoesCatalogo",
                columns: new[] { "Modulo", "Acao", "Recurso" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesAprovacao_AprovadoPorUsuarioId",
                table: "SolicitacoesAprovacao",
                column: "AprovadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesAprovacao_EmpresaId",
                table: "SolicitacoesAprovacao",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesAprovacao_SolicitadoPorUsuarioId",
                table: "SolicitacoesAprovacao",
                column: "SolicitadoPorUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_PerfisAcesso_PerfilAcessoId",
                table: "Usuarios",
                column: "PerfilAcessoId",
                principalTable: "PerfisAcesso",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_PerfisAcesso_PerfilAcessoId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "AuditoriaAcoes");

            migrationBuilder.DropTable(
                name: "ParametrosSistema");

            migrationBuilder.DropTable(
                name: "PerfilPermissoesCatalogo");

            migrationBuilder.DropTable(
                name: "SolicitacoesAprovacao");

            migrationBuilder.DropTable(
                name: "PerfisAcesso");

            migrationBuilder.DropTable(
                name: "PermissoesCatalogo");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PerfilAcessoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PerfilAcessoId",
                table: "Usuarios");
        }
    }
}
