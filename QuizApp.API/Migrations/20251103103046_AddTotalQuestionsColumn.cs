using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalQuestionsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalQuestions",
                table: "QuizResults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalQuestions",
                table: "QuizResults");
        }
    }
}
