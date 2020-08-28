using Microsoft.EntityFrameworkCore.Migrations;

namespace RecipeBox.API.Migrations
{
    public partial class UsernameAddedToPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Posts");
        }
    }
}
