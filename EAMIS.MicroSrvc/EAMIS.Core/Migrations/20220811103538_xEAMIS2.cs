using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class xEAMIS2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ITEM_CODE",
                table: "EAMIS_DELIVERY_RECEIPT_DETAILS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ITEM_DESCRIPTION",
                table: "EAMIS_DELIVERY_RECEIPT_DETAILS",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ITEM_CODE",
                table: "EAMIS_DELIVERY_RECEIPT_DETAILS");

            migrationBuilder.DropColumn(
                name: "ITEM_DESCRIPTION",
                table: "EAMIS_DELIVERY_RECEIPT_DETAILS");
        }
    }
}
