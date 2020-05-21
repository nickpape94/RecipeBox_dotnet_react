using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Dtos
{
    public class PostsForListDto
    {
        public int Id { get; set; }
        public string NameOfDish { get; set; }
        public string Description { get; set; }
        public string PrepTime { get; set; }
        public string CookingTime { get; set; }
        public string Cuisine { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}