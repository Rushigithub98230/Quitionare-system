using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedBy",
                table: "Users",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedBy",
                table: "Users",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestionResponses_CreatedBy",
                table: "UserQuestionResponses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestionResponses_DeletedBy",
                table: "UserQuestionResponses",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestionResponses_UpdatedBy",
                table: "UserQuestionResponses",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTypes_CreatedBy",
                table: "QuestionTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTypes_DeletedBy",
                table: "QuestionTypes",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTypes_UpdatedBy",
                table: "QuestionTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreatedBy",
                table: "Questions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_DeletedBy",
                table: "Questions",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_UpdatedBy",
                table: "Questions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_CreatedBy",
                table: "QuestionResponses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_DeletedBy",
                table: "QuestionResponses",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_UpdatedBy",
                table: "QuestionResponses",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_CreatedBy",
                table: "QuestionOptions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_DeletedBy",
                table: "QuestionOptions",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_UpdatedBy",
                table: "QuestionOptions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptionResponses_CreatedBy",
                table: "QuestionOptionResponses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptionResponses_DeletedBy",
                table: "QuestionOptionResponses",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptionResponses_UpdatedBy",
                table: "QuestionOptionResponses",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplates_CreatedBy",
                table: "QuestionnaireTemplates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplates_DeletedBy",
                table: "QuestionnaireTemplates",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplates_UpdatedBy",
                table: "QuestionnaireTemplates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedBy",
                table: "Categories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_DeletedBy",
                table: "Categories",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UpdatedBy",
                table: "Categories",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_CreatedBy",
                table: "Categories",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_DeletedBy",
                table: "Categories",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UpdatedBy",
                table: "Categories",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireTemplates_Users_CreatedBy",
                table: "QuestionnaireTemplates",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireTemplates_Users_DeletedBy",
                table: "QuestionnaireTemplates",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireTemplates_Users_UpdatedBy",
                table: "QuestionnaireTemplates",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptionResponses_Users_CreatedBy",
                table: "QuestionOptionResponses",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptionResponses_Users_DeletedBy",
                table: "QuestionOptionResponses",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptionResponses_Users_UpdatedBy",
                table: "QuestionOptionResponses",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptions_Users_CreatedBy",
                table: "QuestionOptions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptions_Users_DeletedBy",
                table: "QuestionOptions",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptions_Users_UpdatedBy",
                table: "QuestionOptions",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_Users_CreatedBy",
                table: "QuestionResponses",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_Users_DeletedBy",
                table: "QuestionResponses",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_Users_UpdatedBy",
                table: "QuestionResponses",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_CreatedBy",
                table: "Questions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_DeletedBy",
                table: "Questions",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_UpdatedBy",
                table: "Questions",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTypes_Users_CreatedBy",
                table: "QuestionTypes",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTypes_Users_DeletedBy",
                table: "QuestionTypes",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTypes_Users_UpdatedBy",
                table: "QuestionTypes",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuestionResponses_Users_CreatedBy",
                table: "UserQuestionResponses",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuestionResponses_Users_DeletedBy",
                table: "UserQuestionResponses",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuestionResponses_Users_UpdatedBy",
                table: "UserQuestionResponses",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DeletedBy",
                table: "Users",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UpdatedBy",
                table: "Users",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_CreatedBy",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_DeletedBy",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UpdatedBy",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireTemplates_Users_CreatedBy",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireTemplates_Users_DeletedBy",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireTemplates_Users_UpdatedBy",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptionResponses_Users_CreatedBy",
                table: "QuestionOptionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptionResponses_Users_DeletedBy",
                table: "QuestionOptionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptionResponses_Users_UpdatedBy",
                table: "QuestionOptionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptions_Users_CreatedBy",
                table: "QuestionOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptions_Users_DeletedBy",
                table: "QuestionOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptions_Users_UpdatedBy",
                table: "QuestionOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_Users_CreatedBy",
                table: "QuestionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_Users_DeletedBy",
                table: "QuestionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_Users_UpdatedBy",
                table: "QuestionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_CreatedBy",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_DeletedBy",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_UpdatedBy",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTypes_Users_CreatedBy",
                table: "QuestionTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTypes_Users_DeletedBy",
                table: "QuestionTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTypes_Users_UpdatedBy",
                table: "QuestionTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuestionResponses_Users_CreatedBy",
                table: "UserQuestionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuestionResponses_Users_DeletedBy",
                table: "UserQuestionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuestionResponses_Users_UpdatedBy",
                table: "UserQuestionResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_DeletedBy",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UpdatedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeletedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UpdatedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserQuestionResponses_CreatedBy",
                table: "UserQuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_UserQuestionResponses_DeletedBy",
                table: "UserQuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_UserQuestionResponses_UpdatedBy",
                table: "UserQuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionTypes_CreatedBy",
                table: "QuestionTypes");

            migrationBuilder.DropIndex(
                name: "IX_QuestionTypes_DeletedBy",
                table: "QuestionTypes");

            migrationBuilder.DropIndex(
                name: "IX_QuestionTypes_UpdatedBy",
                table: "QuestionTypes");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CreatedBy",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_DeletedBy",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_UpdatedBy",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionResponses_CreatedBy",
                table: "QuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionResponses_DeletedBy",
                table: "QuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionResponses_UpdatedBy",
                table: "QuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionOptions_CreatedBy",
                table: "QuestionOptions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionOptions_DeletedBy",
                table: "QuestionOptions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionOptions_UpdatedBy",
                table: "QuestionOptions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionOptionResponses_CreatedBy",
                table: "QuestionOptionResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionOptionResponses_DeletedBy",
                table: "QuestionOptionResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionOptionResponses_UpdatedBy",
                table: "QuestionOptionResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireTemplates_CreatedBy",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireTemplates_DeletedBy",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireTemplates_UpdatedBy",
                table: "QuestionnaireTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CreatedBy",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_DeletedBy",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UpdatedBy",
                table: "Categories");
        }
    }
}
