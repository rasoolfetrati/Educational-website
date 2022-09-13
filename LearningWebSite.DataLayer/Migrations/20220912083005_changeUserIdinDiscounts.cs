using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningWebSite.DataLayer.Migrations
{
    public partial class changeUserIdinDiscounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCodes_AspNetUsers_UserId1",
                table: "UserDiscountCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserDiscountCodes_UserId1",
                table: "UserDiscountCodes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserDiscountCodes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserDiscountCodes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountCodes_UserId",
                table: "UserDiscountCodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCodes_AspNetUsers_UserId",
                table: "UserDiscountCodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountCodes_AspNetUsers_UserId",
                table: "UserDiscountCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserDiscountCodes_UserId",
                table: "UserDiscountCodes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserDiscountCodes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserDiscountCodes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountCodes_UserId1",
                table: "UserDiscountCodes",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountCodes_AspNetUsers_UserId1",
                table: "UserDiscountCodes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
