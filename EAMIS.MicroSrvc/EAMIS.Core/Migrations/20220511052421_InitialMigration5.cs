using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class InitialMigration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<string>(
                name: "TIMESTAMP",
                table: "EAMIS_PROPERTY_TRANSACTION",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EAMIS_RESPONSIBILITY_CODE");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "EAMIS_WAREHOUSE");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "EAMIS_UNITOFMEASURE");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "EAMIS_PROCUREMENTCATEGORY");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "EAMIS_ITEM_CATEGORY");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TIMESTAMP",
                table: "EAMIS_PROPERTY_TRANSACTION",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
