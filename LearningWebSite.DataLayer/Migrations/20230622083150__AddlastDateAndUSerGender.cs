using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningWebSite.DataLayer.Migrations
{
    public partial class _AddlastDateAndUSerGender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Courses",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Courses");

            migrationBuilder.AddColumn<byte>(
                name: "Gender",
                table: "AspNetUsers",
                type: "tinyint",
                nullable: true);
        }
    }
}
