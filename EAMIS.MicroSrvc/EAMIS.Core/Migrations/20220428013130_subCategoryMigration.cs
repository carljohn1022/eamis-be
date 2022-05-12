using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class subCategoryMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IS_ACTIVE",
                table: "EAMIS_ITEMS_SUB_CATEGORY",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "EAMIS_PROCUREMENTCATEGORY");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "EAMIS_ITEMS_SUB_CATEGORY");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "EAMIS_ITEM_CATEGORY");
        }
    }
}
