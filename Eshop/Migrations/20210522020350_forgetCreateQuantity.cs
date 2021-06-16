using Microsoft.EntityFrameworkCore.Migrations;

namespace Eshop.Migrations
{
    public partial class forgetCreateQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "InvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InvoiceDetails");
        }
    }
}
