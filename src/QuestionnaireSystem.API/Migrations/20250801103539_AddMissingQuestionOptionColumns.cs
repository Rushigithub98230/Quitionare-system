using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingQuestionOptionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "QuestionOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "QuestionOptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "QuestionOptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "QuestionOptions");
        }
    }
}
