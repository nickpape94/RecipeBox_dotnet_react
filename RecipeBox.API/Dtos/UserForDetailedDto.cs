using System;
using System.Collections.Generic;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos
{
    public class UserForDetailedDto
    {
        public int UserId { get; set; }
        public string Username  { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<UserPhotosForReturnDto> UserPhotos { get; set; }
        public ICollection<PostsForDetailedDto> Posts { get; set; }
        // public ICollection<Favourite> Posts { get; set; }
    }
}