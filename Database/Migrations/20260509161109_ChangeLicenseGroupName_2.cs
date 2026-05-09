using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLicenseGroupName_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseGroups_Users_MaintainerId",
                table: "LicenseGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_LicenseGroups_GroupId",
                table: "Licenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LicenseGroups",
                table: "LicenseGroups");

            migrationBuilder.RenameTable(
                name: "LicenseGroups",
                newName: "Groups");

            migrationBuilder.RenameIndex(
                name: "IX_LicenseGroups_MaintainerId",
                table: "Groups",
                newName: "IX_Groups_MaintainerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_MaintainerId",
                table: "Groups",
                column: "MaintainerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Groups_GroupId",
                table: "Licenses",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_MaintainerId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Groups_GroupId",
                table: "Licenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "LicenseGroups");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_MaintainerId",
                table: "LicenseGroups",
                newName: "IX_LicenseGroups_MaintainerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LicenseGroups",
                table: "LicenseGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseGroups_Users_MaintainerId",
                table: "LicenseGroups",
                column: "MaintainerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_LicenseGroups_GroupId",
                table: "Licenses",
                column: "GroupId",
                principalTable: "LicenseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
