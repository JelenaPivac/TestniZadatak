using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestniZadatak.Migrations
{
    public partial class foreign_key_article_attributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attributes_Article_articleId",
                table: "attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_attributes_AttributeDefinitions_definitionId",
                table: "attributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_attributes",
                table: "attributes");

            migrationBuilder.RenameTable(
                name: "attributes",
                newName: "Attributes");

            migrationBuilder.RenameIndex(
                name: "IX_attributes_definitionId",
                table: "Attributes",
                newName: "IX_Attributes_definitionId");

            migrationBuilder.RenameIndex(
                name: "IX_attributes_articleId",
                table: "Attributes",
                newName: "IX_Attributes_articleId");

            migrationBuilder.AlterColumn<Guid>(
                name: "definitionId",
                table: "Attributes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attributes",
                table: "Attributes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attributes_Article_articleId",
                table: "Attributes",
                column: "articleId",
                principalTable: "Article",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attributes_AttributeDefinitions_definitionId",
                table: "Attributes",
                column: "definitionId",
                principalTable: "AttributeDefinitions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attributes_Article_articleId",
                table: "Attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_Attributes_AttributeDefinitions_definitionId",
                table: "Attributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attributes",
                table: "Attributes");

            migrationBuilder.RenameTable(
                name: "Attributes",
                newName: "attributes");

            migrationBuilder.RenameIndex(
                name: "IX_Attributes_definitionId",
                table: "attributes",
                newName: "IX_attributes_definitionId");

            migrationBuilder.RenameIndex(
                name: "IX_Attributes_articleId",
                table: "attributes",
                newName: "IX_attributes_articleId");

            migrationBuilder.AlterColumn<Guid>(
                name: "definitionId",
                table: "attributes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_attributes",
                table: "attributes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_attributes_Article_articleId",
                table: "attributes",
                column: "articleId",
                principalTable: "Article",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_attributes_AttributeDefinitions_definitionId",
                table: "attributes",
                column: "definitionId",
                principalTable: "AttributeDefinitions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
