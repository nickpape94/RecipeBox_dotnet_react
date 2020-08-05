using System.ComponentModel.DataAnnotations;

namespace RecipeBox.API.Dtos.UserDtos
{
    public class UserEmailForUpdateDto
    {
        [EmailAddress(ErrorMessage = "The email address is not valid")]
        public string Email { get; set; }
    }
}