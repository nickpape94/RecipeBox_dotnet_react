using System;
using System.Collections.Generic;
using RecipeBox.API.Dtos.CommentDtos;
using RecipeBox.API.Dtos.PhotoDtos;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos.PostDtos
{
    public class PostsForListDto
    {
        public int PostId { get; set; }
        public string NameOfDish { get; set; }
        public string Description { get; set; }
        public string PrepTime { get; set; }
        public string CookingTime { get; set; }
        public double AverageRating { get; set; }
        public string Feeds { get; set; }
        public ICollection<RatingsForReturnedDto> Ratings { get; set; }
        public string Cuisine { get; set; }
        public ICollection<CommentsForReturnedDto> Comments { get; set; }
        public ICollection<PostPhotosForReturnDto> PostPhoto { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }
    }
}