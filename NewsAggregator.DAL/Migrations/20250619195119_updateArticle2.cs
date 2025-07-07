using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregator.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateArticle2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "ArticleFeedbacks");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "ArticleFeedbacks");

            migrationBuilder.RenameColumn(
                name: "IsEnable",
                table: "Sources",
                newName: "IsActive");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Sources",
                newName: "IsEnable");

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
    }
}
