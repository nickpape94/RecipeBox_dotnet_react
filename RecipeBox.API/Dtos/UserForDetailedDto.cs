using System;
using System.Collections.Generic;

namespace RecipeBox.API.Dtos
{
    public class UserForDetailedDto
    {
        public int Id { get; set; }
        public string Username  { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<PhotosForDetailedDto> UserPhotos { get; set; }
        public ICollection<PostsForDetailedDto> Posts { get; set; }
    }
}