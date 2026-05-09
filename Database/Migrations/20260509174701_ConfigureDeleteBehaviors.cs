using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_MaintainerId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Groups_GroupId",
                table: "Licenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Users_AssignedToId",
                table: "Seats");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_MaintainerId",
                table: "Groups",
                column: "MaintainerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Groups_GroupId",
                table: "Licenses",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Users_AssignedToId",
                table: "Seats",
                column: "AssignedToId",
                principalTable: "Users",
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

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Users_AssignedToId",
                table: "Seats");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Users_AssignedToId",
                table: "Seats",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
