using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryQuestionnaireRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_QuestionnaireId_DisplayOrder",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireTemplates_CategoryId",
                table: "QuestionnaireTemplates");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowsFollowUpMessaging",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowsMedicationDelivery",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "Categories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ConsultationDescription",
                table: "Categories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Features",
                table: "Categories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMostPopular",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrending",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OneTimeConsultationDurationMinutes",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresQuestionnaireAssessment",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplates_CategoryId",
                table: "QuestionnaireTemplates",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_QuestionnaireId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireTemplates_CategoryId",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropColumn(
                name: "AllowsFollowUpMessaging",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AllowsMedicationDelivery",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ConsultationDescription",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Features",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsMostPopular",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsTrending",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OneTimeConsultationDurationMinutes",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "RequiresQuestionnaireAssessment",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionnaireId_DisplayOrder",
                table: "Questions",
                columns: new[] { "QuestionnaireId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplates_CategoryId",
                table: "QuestionnaireTemplates",
                column: "CategoryId");
        }
    }
}
