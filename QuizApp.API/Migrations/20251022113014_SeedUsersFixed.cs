using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$R5E0uX1h3Q6q9x8T2zE3ROzQs3X6XlV7S1f7QcE5qH9B8u6xYlY1G");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$F3Yp2X7n4Q0r1k8H2bD6S.Ox1kPqXlM9Y4s9PbF3vZ1Q5r6LkT9V2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$eG5hP/9Oy9YFqv5MZk/UKenR5aTzLZQ2M3k9k7/8UuSdsb2yXWg6O");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$1r4qJcK6xZ8U8C5kUuBz0O1vPfXk3KfPjF9oNQkF5L6kJ9ZQy1sG6");
        }
    }
}
