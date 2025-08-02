using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class MakeCreatedByNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireTemplates_Categories_CategoryId",
                table: "QuestionnaireTemplates");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "QuestionnaireTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireTemplates_Categories_CategoryId",
                table: "QuestionnaireTemplates",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireTemplates_Categories_CategoryId",
                table: "QuestionnaireTemplates");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "QuestionnaireTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireTemplates_Categories_CategoryId",
                table: "QuestionnaireTemplates",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
