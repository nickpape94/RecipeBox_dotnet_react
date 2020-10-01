using System;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos.CommentDtos
{
    public class CommentsForReturnedDto
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public User User { get; set; }
        public string Author { get; set; }
        public string UserPhotoUrl { get; set; }
        public int CommenterId { get; set; }
        // public Post Commenter { get; set; }
        // public int PostId { get; set; }
    }
}