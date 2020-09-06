using System;
using System.Collections.Generic;
using RecipeBox.API.Dtos.CommentDtos;
using RecipeBox.API.Dtos.PhotoDtos;

namespace RecipeBox.API.Dtos.PostDtos
{
    public class PostsForDetailedDto
    {
        public int PostId { get; set; }
        public string NameOfDish { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Method { get; set; }
        public string PrepTime { get; set; }
        public string CookingTime { get; set; }
        public string Feeds { get; set; }
        public string Cuisine { get; set; }
        public string Author { get; set; }
        public string UserPhotoUrl { get; set; }
        public ICollection<CommentsForReturnedDto> Comments { get; set; }
        public ICollection<RatingsForReturnedDto> Ratings { get; set; }
        public double AverageRating { get; set; }
        public ICollection<PostPhotosForReturnDto> PostPhoto { get; set; }
        // public int Rating { get; set; }
        public DateTime Created { get; set; }
        public string User { get; set; }
        public int UserId { get; set; }
        
        
    }
}