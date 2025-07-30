using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Triopet.BusinessContext.Migrations
{
    /// <inheritdoc />
    public partial class Recent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ProductExits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ProductEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductExits_ExitId",
                table: "ProductExits",
                column: "ExitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductExits_ProductId",
                table: "ProductExits",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductEntries_EntryId",
                table: "ProductEntries",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductEntries_ProductId",
                table: "ProductEntries",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ProductId",
                table: "Images",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Exits_MotifId",
                table: "Exits",
                column: "MotifId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exits_Motifs_MotifId",
                table: "Exits",
                column: "MotifId",
                principalTable: "Motifs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Products_ProductId",
                table: "Images",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductEntries_Entries_EntryId",
                table: "ProductEntries",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductEntries_Products_ProductId",
                table: "ProductEntries",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductExits_Exits_ExitId",
                table: "ProductExits",
                column: "ExitId",
                principalTable: "Exits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductExits_Products_ProductId",
                table: "ProductExits",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exits_Motifs_MotifId",
                table: "Exits");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Products_ProductId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductEntries_Entries_EntryId",
                table: "ProductEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductEntries_Products_ProductId",
                table: "ProductEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductExits_Exits_ExitId",
                table: "ProductExits");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductExits_Products_ProductId",
                table: "ProductExits");

            migrationBuilder.DropIndex(
                name: "IX_ProductExits_ExitId",
                table: "ProductExits");

            migrationBuilder.DropIndex(
                name: "IX_ProductExits_ProductId",
                table: "ProductExits");

            migrationBuilder.DropIndex(
                name: "IX_ProductEntries_EntryId",
                table: "ProductEntries");

            migrationBuilder.DropIndex(
                name: "IX_ProductEntries_ProductId",
                table: "ProductEntries");

            migrationBuilder.DropIndex(
                name: "IX_Images_ProductId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Exits_MotifId",
                table: "Exits");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductExits");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductEntries");
        }
    }
}
