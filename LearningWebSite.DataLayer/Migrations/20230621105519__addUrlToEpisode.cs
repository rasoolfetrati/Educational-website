using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningWebSite.DataLayer.Migrations
{
    public partial class _addUrlToEpisode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "CourseEpisodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "CourseEpisodes");
        }
    }
}
