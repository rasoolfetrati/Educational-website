using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningWebSite.DataLayer.Migrations
{
    public partial class AddRecommendForCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRecommended",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRecommended",
                table: "Courses");
        }
    }
}
