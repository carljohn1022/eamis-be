using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class InitialMigration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CODE",
                table: "EAMIS_RESPONSIBILITY_CODE",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CODE",
                table: "EAMIS_RESPONSIBILITY_CODE");
        }
    }
}
