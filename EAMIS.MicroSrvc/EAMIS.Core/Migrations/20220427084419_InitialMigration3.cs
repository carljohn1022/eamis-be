using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class InitialMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EAMIS_ATTACHMENTS_PARENTID",
                table: "EAMIS_ATTACHMENTS",
                column: "PARENTID");

            migrationBuilder.AddForeignKey(
                name: "FK_EAMIS_ATTACHMENTS_EAMIS_ATTACHMENTS_PARENTID",
                table: "EAMIS_ATTACHMENTS",
                column: "PARENTID",
                principalTable: "EAMIS_ATTACHMENTS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EAMIS_ATTACHMENTS_EAMIS_ATTACHMENTS_PARENTID",
                table: "EAMIS_ATTACHMENTS");

            migrationBuilder.DropIndex(
                name: "IX_EAMIS_ATTACHMENTS_PARENTID",
                table: "EAMIS_ATTACHMENTS");
        }
    }
}
