using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLicenseGroupName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_LicenseGroups_LicenseGroupId",
                table: "Licenses");

            migrationBuilder.RenameColumn(
                name: "LicenseGroupId",
                table: "Licenses",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Licenses_LicenseGroupId",
                table: "Licenses",
                newName: "IX_Licenses_GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_LicenseGroups_GroupId",
                table: "Licenses",
                column: "GroupId",
                principalTable: "LicenseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_LicenseGroups_GroupId",
                table: "Licenses");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Licenses",
                newName: "LicenseGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Licenses_GroupId",
                table: "Licenses",
                newName: "IX_Licenses_LicenseGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_LicenseGroups_LicenseGroupId",
                table: "Licenses",
                column: "LicenseGroupId",
                principalTable: "LicenseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
