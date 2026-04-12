using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMVEXA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProdutoFotosBytea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var n in new[] { 1, 2, 3, 4, 5 })
            {
                var col = $"Foto{n}";
                var tmp = $"Foto{n}TmpBytea";
                migrationBuilder.AddColumn<byte[]>(
                    name: tmp,
                    table: "Produtos",
                    type: "bytea",
                    nullable: true);

                migrationBuilder.Sql($@"
                    UPDATE ""Produtos""
                    SET ""{tmp}"" = decode(split_part(""{col}"", ',', 2), 'base64')
                    WHERE ""{col}"" IS NOT NULL
                      AND ""{col}"" LIKE 'data:%'
                      AND strpos(""{col}"", ',') > 0;

                    UPDATE ""Produtos""
                    SET ""{tmp}"" = decode(""{col}"", 'base64')
                    WHERE ""{tmp}"" IS NULL
                      AND ""{col}"" IS NOT NULL
                      AND ""{col}"" NOT LIKE '/uploads/%'
                      AND ""{col}"" NOT LIKE 'data:%'
                      AND length(trim(""{col}"")) > 0;
                ");

                migrationBuilder.DropColumn(
                    name: col,
                    table: "Produtos");

                migrationBuilder.RenameColumn(
                    name: tmp,
                    table: "Produtos",
                    newName: col);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var n in new[] { 1, 2, 3, 4, 5 })
            {
                var col = $"Foto{n}";
                var tmp = $"Foto{n}TmpText";
                migrationBuilder.AddColumn<string>(
                    name: tmp,
                    table: "Produtos",
                    type: "text",
                    nullable: true);

                migrationBuilder.Sql($@"
                    UPDATE ""Produtos""
                    SET ""{tmp}"" = 'data:image/jpeg;base64,' || encode(""{col}"", 'base64')
                    WHERE ""{col}"" IS NOT NULL;
                ");

                migrationBuilder.DropColumn(
                    name: col,
                    table: "Produtos");

                migrationBuilder.RenameColumn(
                    name: tmp,
                    table: "Produtos",
                    newName: col);
            }
        }
    }
}
