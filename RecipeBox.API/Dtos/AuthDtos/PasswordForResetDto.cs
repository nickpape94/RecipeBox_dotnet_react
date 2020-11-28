using System.ComponentModel.DataAnnotations;

namespace RecipeBox.API.Dtos.AuthDtos
{
    public class PasswordForResetDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // [Required]
        // [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }

    }
}