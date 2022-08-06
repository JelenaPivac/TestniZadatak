using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestniZadatak.Migrations
{
    public partial class _060820221405 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "articles",
                table: "User",
                newName: "articleIdsJson");

            migrationBuilder.RenameColumn(
                name: "attributes",
                table: "Article",
                newName: "attributesJson");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "articleIdsJson",
                table: "User",
                newName: "articles");

            migrationBuilder.RenameColumn(
                name: "attributesJson",
                table: "Article",
                newName: "attributes");
        }
    }
}
