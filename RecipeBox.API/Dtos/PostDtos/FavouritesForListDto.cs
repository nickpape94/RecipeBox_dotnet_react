using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos.PostDtos
{
    public class FavouritesForListDto
    {
        public int Id { get; set; }
        public int FavouriterId { get; set; }
        public int PostId { get; set; }
    }
}