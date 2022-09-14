using Microsoft.EntityFrameworkCore.Migrations;


#nullable disable

namespace LearningWebSite.DataLayer.Migrations
{
    public partial class AddSlugToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoursePresentation",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Courses",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoursePresentation",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Courses");
        }
    }
}
