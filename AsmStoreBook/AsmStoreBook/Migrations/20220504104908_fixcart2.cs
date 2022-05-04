using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsmStoreBook.Migrations
{
    public partial class fixcart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Cart",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "Cart",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Cart");
        }
    }
}
