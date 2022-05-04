using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsmStoreBook.Migrations
{
    public partial class fixcart1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "Cart",
                newName: "Quantity");

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "OrderDetail",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "Cart",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Cart",
                newName: "quantity");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "Cart",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
