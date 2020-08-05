using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos
{
    public class FavouritesForListDto
    {
        public int Id { get; set; }
        public int FavouriterId { get; set; }
        public int PostId { get; set; }
    }
}