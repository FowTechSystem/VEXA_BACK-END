using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClienteFotoBytea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "Clientes",
                type: "bytea",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""Clientes""
                SET ""Foto"" = decode(split_part(""FotoBase64"", ',', 2), 'base64')
                WHERE ""FotoBase64"" IS NOT NULL
                  AND ""FotoBase64"" LIKE 'data:%'
                  AND strpos(""FotoBase64"", ',') > 0;

                UPDATE ""Clientes""
                SET ""Foto"" = decode(""FotoBase64"", 'base64')
                WHERE ""Foto"" IS NULL
                  AND ""FotoBase64"" IS NOT NULL
                  AND ""FotoBase64"" NOT LIKE '/uploads/%'
                  AND ""FotoBase64"" NOT LIKE 'data:%'
                  AND length(trim(""FotoBase64"")) > 0;
                ");

            migrationBuilder.DropColumn(
                name: "FotoBase64",
                table: "Clientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoBase64",
                table: "Clientes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""Clientes""
                SET ""FotoBase64"" = 'data:image/jpeg;base64,' || encode(""Foto"", 'base64')
                WHERE ""Foto"" IS NOT NULL;
                ");

            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Clientes");
        }
    }
}
