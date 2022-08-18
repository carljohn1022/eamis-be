using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class xEAMIS1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SUB_TOTAL",
                table: "EAMIS_DELIVERY_RECEIPT_DETAILS",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SUB_TOTAL",
                table: "EAMIS_DELIVERY_RECEIPT_DETAILS");
        }
    }
}
