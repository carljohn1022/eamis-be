using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class InitialMigration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EAMIS_EAMISPROPERTY_TRANSACTION",
                table: "EAMIS_EAMISPROPERTY_TRANSACTION");

            migrationBuilder.RenameTable(
                name: "EAMIS_EAMISPROPERTY_TRANSACTION",
                newName: "EAMIS_PROPERTY_TRANSACTION");

            migrationBuilder.RenameColumn(
                name: "ASCALPERION",
                table: "EAMIS_PROPERTY_TRANSACTION",
                newName: "FISCALPERIOD");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EAMIS_PROPERTY_TRANSACTION",
                table: "EAMIS_PROPERTY_TRANSACTION",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EAMIS_PROPERTY_TRANSACTION",
                table: "EAMIS_PROPERTY_TRANSACTION");

            migrationBuilder.RenameTable(
                name: "EAMIS_PROPERTY_TRANSACTION",
                newName: "EAMIS_EAMISPROPERTY_TRANSACTION");

            migrationBuilder.RenameColumn(
                name: "FISCALPERIOD",
                table: "EAMIS_EAMISPROPERTY_TRANSACTION",
                newName: "ASCALPERION");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EAMIS_EAMISPROPERTY_TRANSACTION",
                table: "EAMIS_EAMISPROPERTY_TRANSACTION",
                column: "ID");
        }
    }
}
