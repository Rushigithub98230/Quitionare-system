using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_PatientResponses_ResponseId",
                table: "QuestionResponses");

            migrationBuilder.DropTable(
                name: "PatientResponses");

            migrationBuilder.DropTable(
                name: "PatientQuestionnaireAssignments");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxLength",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValue",
                table: "Questions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinLength",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValue",
                table: "Questions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserQuestionResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    SubmissionIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeTaken = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuestionResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserQuestionResponses_QuestionnaireTemplates_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "QuestionnaireTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserQuestionResponses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplates_CreatedBy",
                table: "QuestionnaireTemplates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestionResponses_QuestionnaireId",
                table: "UserQuestionResponses",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestionResponses_UserId",
                table: "UserQuestionResponses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireTemplates_Users_CreatedBy",
                table: "QuestionnaireTemplates",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_UserQuestionResponses_ResponseId",
                table: "QuestionResponses",
                column: "ResponseId",
                principalTable: "UserQuestionResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireTemplates_Users_CreatedBy",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_UserQuestionResponses_ResponseId",
                table: "QuestionResponses");

            migrationBuilder.DropTable(
                name: "UserQuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireTemplates_CreatedBy",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MaxLength",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MinLength",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "Questions");

            migrationBuilder.CreateTable(
                name: "PatientQuestionnaireAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReminderCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientQuestionnaireAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientQuestionnaireAssignments_QuestionnaireTemplates_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "QuestionnaireTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientQuestionnaireAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmissionIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    TimeTaken = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientResponses_PatientQuestionnaireAssignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "PatientQuestionnaireAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientResponses_QuestionnaireTemplates_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "QuestionnaireTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientResponses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientQuestionnaireAssignments_PatientId",
                table: "PatientQuestionnaireAssignments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientQuestionnaireAssignments_QuestionnaireId",
                table: "PatientQuestionnaireAssignments",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientQuestionnaireAssignments_Status",
                table: "PatientQuestionnaireAssignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PatientQuestionnaireAssignments_UserId",
                table: "PatientQuestionnaireAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientResponses_AssignmentId",
                table: "PatientResponses",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientResponses_QuestionnaireId",
                table: "PatientResponses",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientResponses_UserId",
                table: "PatientResponses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_PatientResponses_ResponseId",
                table: "QuestionResponses",
                column: "ResponseId",
                principalTable: "PatientResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
