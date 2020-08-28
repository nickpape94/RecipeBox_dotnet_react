using System;
using System.Collections.Generic;
using RecipeBox.API.Dtos.PhotoDtos;
using RecipeBox.API.Dtos.PostDtos;

namespace RecipeBox.API.Dtos.UserDtos
{
    public class UserForDetailedDto
    {
        public int Id { get; set; }
        public string Username  { get; set; }
        // public string Email  { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<UserPhotosForReturnDto> UserPhotos { get; set; }
        public ICollection<PostsForDetailedDto> Posts { get; set; }
        // public ICollection<Favourite> Posts { get; set; }
    }
}