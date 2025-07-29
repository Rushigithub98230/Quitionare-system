using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PatientResponses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PatientQuestionnaireAssignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientResponses_UserId",
                table: "PatientResponses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientQuestionnaireAssignments_UserId",
                table: "PatientQuestionnaireAssignments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientQuestionnaireAssignments_Users_UserId",
                table: "PatientQuestionnaireAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientResponses_Users_UserId",
                table: "PatientResponses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientQuestionnaireAssignments_Users_UserId",
                table: "PatientQuestionnaireAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientResponses_Users_UserId",
                table: "PatientResponses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PatientResponses_UserId",
                table: "PatientResponses");

            migrationBuilder.DropIndex(
                name: "IX_PatientQuestionnaireAssignments_UserId",
                table: "PatientQuestionnaireAssignments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PatientResponses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PatientQuestionnaireAssignments");
        }
    }
}
