using System.ComponentModel.DataAnnotations;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos.PostDtos
{
    public class RatePostDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Value must be between 1 and 5")]
        public double Score { get; set; }

        public User User { get; set; }
        public int RaterId { get; set; }
    }
}