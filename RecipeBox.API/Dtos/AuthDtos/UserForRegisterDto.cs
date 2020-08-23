using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeBox.API.Dtos.AuthDtos
{
    public class UserForRegisterDto
    {

        public string Email { get; set; }

        public string Username { get; set; }
        
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