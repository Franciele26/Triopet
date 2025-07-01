using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triopet.BusinessContext.Migrations
{
    /// <inheritdoc />
    public partial class RecentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductExits",
                table: "ProductExits");

            migrationBuilder.DropIndex(
                name: "IX_ProductExits_ExitId",
                table: "ProductExits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductEntries",
                table: "ProductEntries");

            migrationBuilder.DropIndex(
                name: "IX_ProductEntries_EntryId",
                table: "ProductEntries");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductExits");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductExits",
                table: "ProductExits",
                columns: new[] { "ExitId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductEntries",
                table: "ProductEntries",
                columns: new[] { "EntryId", "ProductId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductExits",
                table: "ProductExits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductEntries",
                table: "ProductEntries");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductExits",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductEntries",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductExits",
                table: "ProductExits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductEntries",
                table: "ProductEntries",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductExits_ExitId",
                table: "ProductExits",
                column: "ExitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductEntries_EntryId",
                table: "ProductEntries",
                column: "EntryId");
        }
    }
}
