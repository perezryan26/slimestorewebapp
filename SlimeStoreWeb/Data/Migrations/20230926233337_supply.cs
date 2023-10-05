using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlimeStoreWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class supply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Supply",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Supply",
                table: "Items");
        }
    }
}
