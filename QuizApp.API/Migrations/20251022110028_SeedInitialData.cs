using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuizApp.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Math" },
                    { 2, "Science" }
                });

            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "EndTime", "StartTime", "Title" },
                values: new object[] { 1, new DateTime(2025, 10, 22, 11, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), "Sample Quiz" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 22, 10, 0, 0, 0, DateTimeKind.Utc), "$2a$11$STATIC_HASH_FOR_ADMIN", "Admin", "admin" },
                    { 2, new DateTime(2025, 10, 22, 10, 0, 0, 0, DateTimeKind.Utc), "$2a$11$STATIC_HASH_FOR_NEERAJ", "User", "Neeraj" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "CategoryId", "Text" },
                values: new object[,]
                {
                    { 1, 1, "2 + 2 = ?" },
                    { 2, 2, "H2O is known as?" }
                });

            migrationBuilder.InsertData(
                table: "Options",
                columns: new[] { "Id", "IsCorrect", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 1, false, 1, "3" },
                    { 2, true, 1, "4" },
                    { 3, true, 2, "Water" },
                    { 4, false, 2, "Oxygen" }
                });

            migrationBuilder.InsertData(
                table: "QuizQuestions",
                columns: new[] { "QuestionId", "QuizId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Options",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Options",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Options",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Options",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "QuizQuestions",
                keyColumns: new[] { "QuestionId", "QuizId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "QuizQuestions",
                keyColumns: new[] { "QuestionId", "QuizId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
