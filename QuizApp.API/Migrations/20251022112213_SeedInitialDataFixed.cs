using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialDataFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "$2a$11$eG5hP/9Oy9YFqv5MZk/UKenR5aTzLZQ2M3k9k7/8UuSdsb2yXWg6O" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "$2a$11$1r4qJcK6xZ8U8C5kUuBz0O1vPfXk3KfPjF9oNQkF5L6kJ9ZQy1sG6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 22, 10, 0, 0, 0, DateTimeKind.Utc), "$2a$11$STATIC_HASH_FOR_ADMIN" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 22, 10, 0, 0, 0, DateTimeKind.Utc), "$2a$11$STATIC_HASH_FOR_NEERAJ" });
        }
    }
}
