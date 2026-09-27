using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShopAPI.Migrations
{
    /// <inheritdoc />
    public partial class validtopromotable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Promotions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Promotions");
        }
    }
}
