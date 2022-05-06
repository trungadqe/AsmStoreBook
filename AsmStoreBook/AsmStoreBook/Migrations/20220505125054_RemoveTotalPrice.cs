using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsmStoreBook.Migrations
{
    public partial class RemoveTotalPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Cart");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Cart",
                type: "float",
                nullable: true);
        }
    }
}
