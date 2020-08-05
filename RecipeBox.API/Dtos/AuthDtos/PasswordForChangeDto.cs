using System.ComponentModel.DataAnnotations;

namespace RecipeBox.API.Dtos
{
    public class PasswordForChangeDto
    {
        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "You must specify a password between 6 and 20 characters")]
        public string NewPassword { get; set; }
    }
}