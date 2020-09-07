using System;
using System.ComponentModel.DataAnnotations;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos.PostDtos
{
    public class PostForCreationDto
    {
        // public int PostId { get; set; }
        
        [Required]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Name of dish must be longer than 3 characters.")]
        public string NameOfDish { get; set; }
        
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 500 characters long.")]
        public string Description { get; set; }
        
        [Required]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Ingredients must be between 5 and 500 characters long.")]
        public string Ingredients { get; set; }

        [Required]
        [StringLength(2500, MinimumLength = 5, ErrorMessage = "Method must be between 5 and 2500 characters long.")]
        public string Method { get; set; }
        
        [Required]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Prep time must be longer than 1 character.")]
        public string PrepTime { get; set; }
       
        [Required]       
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Cooking time must be longer than 1 character")]
        public string CookingTime { get; set; }
        
        [Required]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Number of people this recipe feeds must be longer than 1 character")]
        public string Feeds { get; set; }
        
        // [StringLength(35, MinimumLength = 2, ErrorMessage = "Cuisine must be between 2 and 35 characters long")]
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