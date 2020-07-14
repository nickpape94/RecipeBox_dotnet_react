using System;
using System.ComponentModel.DataAnnotations;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos
{
    public class PostForCreationDto
    {
        // public int PostId { get; set; }
        
        [Required]
        [StringLength(60, MinimumLength = 5, ErrorMessage = "Name of dish must be between 5 and 60 characters long")]
        public string NameOfDish { get; set; }
        
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 300 characters long")]
        public string Description { get; set; }
        
        [Required]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Ingredients must be between 5 and 500 characters long")]
        public string Ingredients { get; set; }

        [Required]
        [StringLength(2500, MinimumLength = 5, ErrorMessage = "Method must be between 5 and 2500 characters long")]
        public string Method { get; set; }
        
        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Prep time must be between 5 and 30 characters long")]
        public string PrepTime { get; set; }
       
        [Required]       
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Cooking time must be between 5 and 30 characters long")]
        public string CookingTime { get; set; }
        
        [Required]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Number of people this recipe feeds must be between 5 and 30 characters long")]
        public string Feeds { get; set; }
        
        [StringLength(35, MinimumLength = 2, ErrorMessage = "Cuisine must be between 2 and 35 characters long")]
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