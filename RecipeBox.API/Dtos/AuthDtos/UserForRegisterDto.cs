using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeBox.API.Dtos
{
    public class UserForRegisterDto
    {

        [EmailAddress(ErrorMessage = "The email address is not valid")]
        public string Email { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "You must specify a valid username")]
        public string Username { get; set; }
        
        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "You must specify a password between 6 and 20 characters")]
        public string Password { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime LastActive { get; set; }
        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now; 
        }
    }
}