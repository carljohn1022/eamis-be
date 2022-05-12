using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class InitialMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EAMIS_ATTACHMENT_TYPE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ATTACHMENTSID = table.Column<int>(type: "int", nullable: true),
                    ATTACHMENT_ID = table.Column<int>(type: "int", nullable: false),
                    ATTACHMENT_TYPE_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAMIS_ATTACHMENT_TYPE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EAMIS_ATTACHMENT_TYPE_EAMIS_ATTACHMENTS_ATTACHMENTSID",
                        column: x => x.ATTACHMENTSID,
                        principalTable: "EAMIS_ATTACHMENTS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EAMIS_ATTACHMENT_TYPE_ATTACHMENTSID",
                table: "EAMIS_ATTACHMENT_TYPE",
                column: "ATTACHMENTSID");
        }
    }
}
