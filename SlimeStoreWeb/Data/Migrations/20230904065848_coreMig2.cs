using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlimeStoreWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class coreMig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerItem_Items_ItemsId",
                table: "CustomerItem");

            migrationBuilder.RenameColumn(
                name: "ItemsId",
                table: "CustomerItem",
                newName: "FavoriteItemsId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerItem_ItemsId",
                table: "CustomerItem",
                newName: "IX_CustomerItem_FavoriteItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerItem_Items_FavoriteItemsId",
                table: "CustomerItem",
                column: "FavoriteItemsId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerItem_Items_FavoriteItemsId",
                table: "CustomerItem");

            migrationBuilder.RenameColumn(
                name: "FavoriteItemsId",
                table: "CustomerItem",
                newName: "ItemsId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerItem_FavoriteItemsId",
                table: "CustomerItem",
                newName: "IX_CustomerItem_ItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerItem_Items_ItemsId",
                table: "CustomerItem",
                column: "ItemsId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
