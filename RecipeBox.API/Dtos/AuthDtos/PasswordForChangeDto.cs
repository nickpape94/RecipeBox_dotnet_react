using System.ComponentModel.DataAnnotations;

namespace RecipeBox.API.Dtos.AuthDtos
{
    public class PasswordForChangeDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "You must specify a password between 6 and 20 characters")]
        public string NewPassword { get; set; }
        // public string ConfirmPassword { get; set; }

    }
}