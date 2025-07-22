using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregator.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedArticleReportCountPropertyAndAddedArticleFeedbackIsReportedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReported",
                table: "ArticleFeedbacks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "IsReported",
                table: "ArticleFeedbacks");
        }
    }
}
