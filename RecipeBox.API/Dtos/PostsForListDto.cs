using System.Collections.Generic;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos
{
    public class PostsForListDto
    {
        public int PostId { get; set; }
        public string NameOfDish { get; set; }
        public string Description { get; set; }
        public string PrepTime { get; set; }
        public string CookingTime { get; set; }
        public string Cuisine { get; set; }
        public ICollection<PostPhotosForReturnDto> PostPhoto { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}