using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EAMIS_ATTACHMENTS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PARENTID = table.Column<int>(type: "int", nullable: false),
                    ATTACHMENT_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ATTACHMENT_TYPE_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IS_REQUIRED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAMIS_ATTACHMENTS", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EAMIS_APP_USER_INFO");

            migrationBuilder.DropTable(
                name: "EAMIS_ATTACHMENT_TYPE");

            migrationBuilder.DropTable(
                name: "EAMIS_ENVIRONMENTALASPECTS");

            migrationBuilder.DropTable(
                name: "EAMIS_ENVIRONMENTALIMPACTS");

            migrationBuilder.DropTable(
                name: "EAMIS_FUND_SOURCE");

            migrationBuilder.DropTable(
                name: "EAMIS_ITEMS_SUB_CATEGORY");

            migrationBuilder.DropTable(
                name: "EAMIS_NOTICE_OF_DELIVERY");

            migrationBuilder.DropTable(
                name: "EAMIS_PPECONDITIONS");

            migrationBuilder.DropTable(
                name: "EAMIS_PROCUREMENTCATEGORY");

            migrationBuilder.DropTable(
                name: "EAMIS_PROPERTY_DEPRECIATION");

            migrationBuilder.DropTable(
                name: "EAMIS_PROPERTY_DETAILS");

            migrationBuilder.DropTable(
                name: "EAMIS_PROPERTY_TRANSFER_DETAILS");

            migrationBuilder.DropTable(
                name: "EAMIS_PROPERTY_TYPE");

            migrationBuilder.DropTable(
                name: "EAMIS_USER_LOGIN");

            migrationBuilder.DropTable(
                name: "EAMIS_USER_ROLES");

            migrationBuilder.DropTable(
                name: "EAMIS_ATTACHMENTS");

            migrationBuilder.DropTable(
                name: "EAMIS_AUTHORIZATION");

            migrationBuilder.DropTable(
                name: "EAMIS_FINANCING_SOURCE");

            migrationBuilder.DropTable(
                name: "EAMIS_GENERAL_FUND_SOURCE");

            migrationBuilder.DropTable(
                name: "EAMIS_PROPERTYITEMS");

            migrationBuilder.DropTable(
                name: "EAMIS_ROLES");

            migrationBuilder.DropTable(
                name: "EAMIS_USERS");

            migrationBuilder.DropTable(
                name: "EAMIS_ITEM_CATEGORY");

            migrationBuilder.DropTable(
                name: "EAMIS_SUPPLIER");

            migrationBuilder.DropTable(
                name: "EAMIS_UNITOFMEASURE");

            migrationBuilder.DropTable(
                name: "EAMIS_WAREHOUSE");

            migrationBuilder.DropTable(
                name: "EAMIS_CHART_OF_ACCOUNTS");

            migrationBuilder.DropTable(
                name: "EAMIS_BARANGAY");

            migrationBuilder.DropTable(
                name: "EAMIS_GENERAL_CHART_OF_ACCOUNTS");

            migrationBuilder.DropTable(
                name: "EAMIS_GROUP_CLASSIFICATION");

            migrationBuilder.DropTable(
                name: "EAMIS_MUNICIPALITY");

            migrationBuilder.DropTable(
                name: "EAMIS_SUB_CLASSIFICATION");

            migrationBuilder.DropTable(
                name: "EAMIS_PROVINCE");

            migrationBuilder.DropTable(
                name: "EAMIS_CLASSIFICATION");

            migrationBuilder.DropTable(
                name: "EAMIS_REGION");
        }
    }
}
