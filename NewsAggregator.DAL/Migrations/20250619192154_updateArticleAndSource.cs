using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregator.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateArticleAndSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Articles");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnable",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessedDate",
                table: "Sources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "ArticleFeedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "ArticleFeedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "IsEnable",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "LastAccessedDate",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "ArticleFeedbacks");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "ArticleFeedbacks");

            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
