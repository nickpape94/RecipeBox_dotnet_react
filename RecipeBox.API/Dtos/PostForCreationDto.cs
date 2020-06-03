using System;
using System.ComponentModel.DataAnnotations;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos
{
    public class PostForCreationDto
    {
        
        [Required]
        public string NameOfDish { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Method { get; set; }
        
        [Required]
        public string PrepTime { get; set; }
       
        [Required]       
        public string CookingTime { get; set; }
        
        [Required]
        public string Feeds { get; set; }
        
        public string Cuisine { get; set; }
        
        public DateTime Created { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }
        
        public PostForCreationDto()
        {
            Created = DateTime.Now;
        }
    }
}