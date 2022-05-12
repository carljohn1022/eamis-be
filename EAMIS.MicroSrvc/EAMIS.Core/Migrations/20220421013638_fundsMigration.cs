using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EAMIS.Core.Migrations
{
    public partial class fundsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EAMIS_AUTHORIZATION",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AUTHORIZATION_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAMIS_AUTHORIZATION", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EAMIS_FINANCING_SOURCE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FINANCING_SOURCE_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAMIS_FINANCING_SOURCE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EAMIS_FUND_SOURCE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GENERAL_FUND_SOURCE_ID = table.Column<int>(type: "int", nullable: false),
                    FINANCING_SOURCE_ID = table.Column<int>(type: "int", nullable: false),
                    AUTHORIZATION_ID = table.Column<int>(type: "int", nullable: false),
                    FUND_CATEGORY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAMIS_FUND_SOURCE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EAMIS_FUND_SOURCE_EAMIS_AUTHORIZATION_AUTHORIZATION_ID",
                        column: x => x.AUTHORIZATION_ID,
                        principalTable: "EAMIS_AUTHORIZATION",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EAMIS_FUND_SOURCE_EAMIS_FINANCING_SOURCE_FINANCING_SOURCE_ID",
                        column: x => x.FINANCING_SOURCE_ID,
                        principalTable: "EAMIS_FINANCING_SOURCE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EAMIS_FUND_SOURCE_EAMIS_GENERAL_FUND_SOURCE_GENERAL_FUND_SOURCE_ID",
                        column: x => x.GENERAL_FUND_SOURCE_ID,
                        principalTable: "EAMIS_GENERAL_FUND_SOURCE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EAMIS_FUND_SOURCE_AUTHORIZATION_ID",
                table: "EAMIS_FUND_SOURCE",
                column: "AUTHORIZATION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EAMIS_FUND_SOURCE_FINANCING_SOURCE_ID",
                table: "EAMIS_FUND_SOURCE",
                column: "FINANCING_SOURCE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EAMIS_FUND_SOURCE_GENERAL_FUND_SOURCE_ID",
                table: "EAMIS_FUND_SOURCE",
                column: "GENERAL_FUND_SOURCE_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EAMIS_APP_USER_INFO");

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
                name: "EAMIS_REQUIRED_ATTACHMENTS");

            migrationBuilder.DropTable(
                name: "EAMIS_USER_LOGIN");

            migrationBuilder.DropTable(
                name: "EAMIS_USER_ROLES");

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
