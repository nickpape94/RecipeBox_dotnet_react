using Microsoft.EntityFrameworkCore.Migrations;

namespace RecipeBox.API.Migrations
{
    public partial class UserPhotoToPostModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserPhotoUrl",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPhotoUrl",
                table: "Posts");
        }
    }
}
