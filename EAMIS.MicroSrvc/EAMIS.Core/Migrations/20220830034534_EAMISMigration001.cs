using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class EAMISMigration001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         

            migrationBuilder.AlterColumn<string>(
                name: "ACCOUNT_NUMBER",
                table: "EAMIS_SUPPLIER",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

     
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EAMIS_ATTACHED_FILES");

            migrationBuilder.DropTable(
                name: "EAMIS_FACTOR_TYPE");

            migrationBuilder.DropTable(
                name: "EAMIS_PROPERTY_REVALUATION");

            migrationBuilder.DropTable(
                name: "EAMIS_PROPERTY_REVALUATION_DETAILS");

            migrationBuilder.DropTable(
                name: "EAMIS_PROPERTY_SCHEDULE");

            migrationBuilder.DropTable(
                name: "EAMIS_SERIAL_TRAN");

            migrationBuilder.DropColumn(
                name: "IMG_URL",
                table: "EAMIS_PROPERTYITEMS");

            migrationBuilder.DropColumn(
                name: "FUND_SOURCE",
                table: "EAMIS_PROPERTY_TRANSACTION");

            migrationBuilder.DropColumn(
                name: "DEPRECIATION_MONTH",
                table: "EAMIS_PROPERTY_DEPRECIATION");

            migrationBuilder.DropColumn(
                name: "DEPRECIATION_YEAR",
                table: "EAMIS_PROPERTY_DEPRECIATION");

            migrationBuilder.RenameColumn(
                name: "PROPERTY_SCHEDULE_ID",
                table: "EAMIS_PROPERTY_DEPRECIATION",
                newName: "PROPERTY_DETAILS_ID");

            migrationBuilder.RenameColumn(
                name: "POSTING_DATE",
                table: "EAMIS_PROPERTY_DEPRECIATION",
                newName: "DEPRECIATION_DATE");

            migrationBuilder.AlterColumn<int>(
                name: "ACCOUNT_NUMBER",
                table: "EAMIS_SUPPLIER",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TOTAL_AMOUNT",
                table: "EAMIS_DELIVERY_RECEIPT",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
