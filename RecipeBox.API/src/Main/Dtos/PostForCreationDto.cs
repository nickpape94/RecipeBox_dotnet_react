using System;
using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Dtos
{
    public class PostForCreationDto
    {
        public string NameOfDish { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Method { get; set; }
        public string PrepTime { get; set; }
        public string CookingTime { get; set; }
        public string Feeds { get; set; }
        public string Cuisine { get; set; }
        public DateTime Created { get; set; }

        public User User { get; set; }
        public int? UserId { get; set; }
        
        public PostForCreationDto()
        {
            Created = DateTime.Now;
        }
    }
}