using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecipeBox.API.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class updatedCommentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPhotoUrl",
                table: "Comments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserPhotoUrl",
                table: "Comments");
        }
    }
}
